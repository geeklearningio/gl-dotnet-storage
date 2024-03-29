﻿using System.ComponentModel.Design;
using Azure.Identity;

namespace GeekLearning.Storage.Azure
{
    using GeekLearning.Storage.Azure.Configuration;
    using global::Azure;
    using global::Azure.Storage.Blobs;
    using global::Azure.Storage.Blobs.Models;
    using global::Azure.Storage.Sas;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    public class AzureStore : IStore
    {
        private readonly AzureStoreOptions storeOptions;
        private readonly Lazy<BlobContainerClient> container;

        public AzureStore(AzureStoreOptions storeOptions)
        {
            storeOptions.Validate();

            this.storeOptions = storeOptions;
            this.container = new Lazy<BlobContainerClient>(() =>
                storeOptions.AuthenticationMode != "DefaultAzureCredential"
                    ? new BlobContainerClient(storeOptions.ConnectionString, storeOptions.FolderName)
                    : new BlobContainerClient(new Uri($"{storeOptions.ConnectionString}{storeOptions.FolderName}/"),
                        new DefaultAzureCredential()));
        }

        public string Name => this.storeOptions.Name;

        public Task InitAsync()
        {
            PublicAccessType accessType;
            switch (this.storeOptions.AccessLevel)
            {
                case Storage.Configuration.AccessLevel.Public:
                    accessType = PublicAccessType.BlobContainer;
                    break;
                case Storage.Configuration.AccessLevel.Confidential:
                    accessType = PublicAccessType.Blob;
                    break;
                case Storage.Configuration.AccessLevel.Private:
                default:
                    accessType = PublicAccessType.None;
                    break;
            }

            return this.container.Value.CreateIfNotExistsAsync(accessType, null, null);
        }

        public async ValueTask<IFileReference[]> ListAsync(string path, bool recursive, bool withMetadata)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = null;
            }
            else
            {
                if (!path.EndsWith("/"))
                {
                    path = path + "/";
                }
            }

            var results = recursive ? await RetrieveRecursive(path, withMetadata) : await Retrieve(path, withMetadata);

            return results.Select(blob =>
                    new Internal.AzureFileReference(this.container.Value, storeOptions, blob,
                        withMetadata: withMetadata))
                .ToArray();
        }

        private async Task<List<BlobItem>> RetrieveRecursive(string path, bool withMetadata)
        {
            List<BlobItem> results = new List<BlobItem>();
            var resultSegment =
                this.container.Value
                    .GetBlobsAsync(withMetadata ? BlobTraits.Metadata : BlobTraits.None, BlobStates.None, prefix: path)
                    .AsPages(default, 1000);

            await foreach (Page<BlobItem> blobPage in resultSegment)
            {
                results.AddRange(blobPage.Values);
            }

            return results;
        }

        private async Task<List<BlobItem>> Retrieve(string path, bool withMetadata)
        {
            List<BlobItem> results = new List<BlobItem>();
            var resultSegment =
                this.container.Value
                    .GetBlobsByHierarchyAsync(withMetadata ? BlobTraits.Metadata : BlobTraits.None, BlobStates.None,
                        delimiter: "/", prefix: path)
                    .AsPages(default, 1000);

            await foreach (Page<BlobHierarchyItem> blobPage in resultSegment)
            {
                results.AddRange(
                    blobPage.Values
                        .Where(item => item.IsBlob)
                        .Select(item => item.Blob));
            }

            return results;
        }

        public async ValueTask<IFileReference[]> ListAsync(string path, string searchPattern, bool recursive,
            bool withMetadata)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = null;
            }
            else
            {
                if (!path.EndsWith("/"))
                {
                    path = path + "/";
                }
            }

            string prefix = path;
            var firstWildCard = searchPattern.IndexOf('*');
            if (firstWildCard >= 0)
            {
                prefix += searchPattern.Substring(0, firstWildCard);
                searchPattern = searchPattern.Substring(firstWildCard);
            }

            Microsoft.Extensions.FileSystemGlobbing.Matcher matcher =
                new Microsoft.Extensions.FileSystemGlobbing.Matcher(StringComparison.Ordinal);
            matcher.AddInclude(searchPattern);

            var results = recursive
                ? await RetrieveRecursive(prefix, withMetadata)
                : await Retrieve(prefix, withMetadata);

            var pathMap = results
                .Select(blob => new Internal.AzureFileReference(this.container.Value, storeOptions, blob, withMetadata: withMetadata))
                .ToDictionary(x => Path.GetFileName(x.Path));

            var filteredResults = matcher.Execute(new Internal.AzureListDirectoryWrapper(path, pathMap));

            return filteredResults.Files.Select(x => pathMap[x.Path]).ToArray();
        }

        public async ValueTask<IFileReference> GetAsync(IPrivateFileReference file, bool withMetadata)
        {
            return await this.InternalGetAsync(file, withMetadata);
        }

        public async ValueTask<IFileReference> GetAsync(Uri uri, bool withMetadata)
        {
            return await this.InternalGetAsync(uri, withMetadata);
        }

        public async Task DeleteAsync(IPrivateFileReference file)
        {
            var fileReference = await this.InternalGetAsync(file);
            await fileReference.DeleteAsync();
        }

        public async ValueTask<Stream> ReadAsync(IPrivateFileReference file)
        {
            var fileReference = await this.InternalGetAsync(file);
            return await fileReference.ReadInMemoryAsync();
        }

        public async ValueTask<byte[]> ReadAllBytesAsync(IPrivateFileReference file)
        {
            var fileReference = await this.InternalGetAsync(file);
            return await fileReference.ReadAllBytesAsync();
        }

        public async ValueTask<string> ReadAllTextAsync(IPrivateFileReference file)
        {
            var fileReference = await this.InternalGetAsync(file);
            return await fileReference.ReadAllTextAsync();
        }

        public async ValueTask<IFileReference> SaveAsync(byte[] data, IPrivateFileReference file, string contentType,
            OverwritePolicy overwritePolicy = OverwritePolicy.Always, IDictionary<string, string> metadata = null)
        {
            using (var stream = new MemoryStream(data, 0, data.Length))
            {
                return await this.SaveAsync(stream, file, contentType, overwritePolicy, metadata);
            }
        }

        public async ValueTask<IFileReference> SaveAsync(Stream data, IPrivateFileReference file, string contentType,
            OverwritePolicy overwritePolicy = OverwritePolicy.Always, IDictionary<string, string> metadata = null)
        {
            var uploadBlob = true;
            var blockBlob = this.container.Value.GetBlobClient(file.Path);
            var blobExists = await blockBlob.ExistsAsync();

            Response<BlobProperties> blobProperties = null;

            if (blobExists)
            {
                if (overwritePolicy == OverwritePolicy.Never)
                {
                    throw new Exceptions.FileAlreadyExistsException(this.Name, file.Path);
                }

                blobProperties = await blockBlob.GetPropertiesAsync();

                if (overwritePolicy == OverwritePolicy.IfContentModified)
                {
                    using (var md5 = MD5.Create())
                    {
                        data.Seek(0, SeekOrigin.Begin);
                        var contentMD5 = Convert.ToBase64String(md5.ComputeHash(data));
                        data.Seek(0, SeekOrigin.Begin);
                        uploadBlob = contentMD5 != Convert.ToBase64String(blobProperties.Value.ContentHash);
                    }
                }
            }

            var isModified = false;

            if (uploadBlob)
            {
                await blockBlob.UploadAsync(data, overwrite: true);

                if (blobProperties == null || blobProperties.Value.ContentType != contentType)
                {
                    blobProperties = await blockBlob.GetPropertiesAsync();
                    await blockBlob.SetHttpHeadersAsync(new BlobHttpHeaders
                    {
                        ContentType = contentType,
                        CacheControl = blobProperties.Value.CacheControl,
                        ContentDisposition = blobProperties.Value.ContentDisposition,
                        ContentEncoding = blobProperties.Value.ContentEncoding,
                        ContentHash = blobProperties.Value.ContentHash,
                    });
                }

                isModified = true;
            }

            if (metadata != null)
            {
                await blockBlob.SetMetadataAsync(metadata);
                isModified = true;
            }

            if (isModified)
            {
                blobProperties = await blockBlob.GetPropertiesAsync();
            }

            return new Internal.AzureFileReference(container.Value, this.storeOptions, blockBlob.Name, blobProperties.Value);
        }

        public ValueTask<string> GetSharedAccessSignatureAsync(ISharedAccessPolicy policy)
        {
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = storeOptions.FolderName,
                Resource = "c",
            };

            if (policy.StartTime.HasValue)
            {
                sasBuilder.StartsOn = policy.StartTime.Value;
            }

            if (policy.ExpiryTime.HasValue)
            {
                sasBuilder.ExpiresOn = policy.ExpiryTime.Value;
            }

            sasBuilder.SetPermissions(FromGenericToAzure(policy.Permissions));

            return new ValueTask<string>(this.container.Value.GenerateSasUri(sasBuilder).Query);
        }

        internal static BlobSasPermissions FromGenericToAzure(SharedAccessPermissions permissions)
        {
            var result = (BlobSasPermissions) 0;

            if (permissions.HasFlag(SharedAccessPermissions.Add))
            {
                result |= BlobSasPermissions.Add;
            }

            if (permissions.HasFlag(SharedAccessPermissions.Create))
            {
                result |= BlobSasPermissions.Create;
            }

            if (permissions.HasFlag(SharedAccessPermissions.Delete))
            {
                result |= BlobSasPermissions.Delete;
            }

            if (permissions.HasFlag(SharedAccessPermissions.List))
            {
                result |= BlobSasPermissions.List;
            }

            if (permissions.HasFlag(SharedAccessPermissions.Read))
            {
                result |= BlobSasPermissions.Read;
            }

            if (permissions.HasFlag(SharedAccessPermissions.Write))
            {
                result |= BlobSasPermissions.Write;
            }

            return result;
        }

        private ValueTask<Internal.AzureFileReference> InternalGetAsync(IPrivateFileReference file,
            bool withMetadata = false)
        {
            return this.InternalGetAsync(new Uri(file.Path, UriKind.Relative), withMetadata);
        }

        private async ValueTask<Internal.AzureFileReference> InternalGetAsync(Uri uri, bool withMetadata)
        {
            var path = uri.IsAbsoluteUri ? uri.AbsolutePath : uri.ToString();

            try
            {
                BlobClient blobClient = this.container.Value.GetBlobClient(path);

                if (!withMetadata)
                {
                    if (!(await blobClient.ExistsAsync()))
                    {
                        return null;
                    }
                }

                return new Internal.AzureFileReference(this.container.Value, this.storeOptions, path,
                    await blobClient.GetPropertiesAsync());
            }
            catch (RequestFailedException storageException)
            {
                if (storageException.Status == 404)
                {
                    return null;
                }

                throw;
            }
        }
    }
}