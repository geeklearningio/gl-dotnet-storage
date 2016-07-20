namespace GeekLearning.Storage.Azure
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class AzureStore : IStore
    {
        private string connectionString;
        private Lazy<CloudBlobContainer> container;
        private Lazy<CloudBlobClient> client;
        private string containerName;

        public AzureStore(string storeName, string connectionString, string containerName)
        {
            this.Name = storeName;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentNullException("containerName");
            }

            this.connectionString = connectionString;
            this.containerName = containerName;

            client = new Lazy<CloudBlobClient>(() => CloudStorageAccount.Parse(this.connectionString).CreateCloudBlobClient());
            container = new Lazy<CloudBlobContainer>(() => this.client.Value.GetContainerReference(this.containerName));
        }

        public string Name { get; }

        private async Task<Internal.AzureFileReference> InternalGetAsync(IPrivateFileReference file)
        {
            var azureFile = file as Internal.AzureFileReference;
            if (azureFile != null)
            {
                return azureFile;
            }

            try
            {
                var blob = await this.container.Value.GetBlobReferenceFromServerAsync(file.Path);
                return new Internal.AzureFileReference(file.Path, blob);
            }
            catch (StorageException storageException)
            {
                if (storageException.RequestInformation.HttpStatusCode == 404)
                {
                    return null;
                }
                throw;
            }
        }

        public async Task<IFileReference> GetAsync(IPrivateFileReference file)
        {
            return await InternalGetAsync(file);
        }

        public async Task<IFileReference> GetAsync(Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                return new Internal.AzureFileReference(await this.client.Value.GetBlobReferenceFromServerAsync(uri));
            }
            else
            {
                return new Internal.AzureFileReference(await this.container.Value.GetBlobReferenceFromServerAsync(uri.ToString()));
            }
        }


        public async Task<Stream> ReadAsync(IPrivateFileReference file)
        {
            var fileReference = await InternalGetAsync(file);
            return await fileReference.ReadInMemoryAsync();
        }

        public async Task<byte[]> ReadAllBytesAsync(IPrivateFileReference file)
        {
            var fileReference = await InternalGetAsync(file);
            return await fileReference.ReadAllBytesAsync();
        }

        public async Task<string> ReadAllTextAsync(IPrivateFileReference file)
        {
            var fileReference = await InternalGetAsync(file);
            return await fileReference.ReadAllTextAsync();
        }

        public async Task<IFileReference> SaveAsync(Stream data, IPrivateFileReference file, string mimeType)
        {
            var blockBlob = this.container.Value.GetBlockBlobReference(file.Path);
            await blockBlob.UploadFromStreamAsync(data);
            blockBlob.Properties.ContentType = mimeType;
            blockBlob.Properties.CacheControl = "max-age=300, must-revalidate";
            await blockBlob.SetPropertiesAsync();
            return new Internal.AzureFileReference(blockBlob);
        }

        public async Task<IFileReference> SaveAsync(byte[] data, IPrivateFileReference file, string mimeType)
        {
            var blockBlob = this.container.Value.GetBlockBlobReference(file.Path);
            await blockBlob.UploadFromByteArrayAsync(data, 0, data.Length);
            blockBlob.Properties.ContentType = mimeType;
            blockBlob.Properties.CacheControl = "max-age=300, must-revalidate";
            await blockBlob.SetPropertiesAsync();
            return new Internal.AzureFileReference(blockBlob);
        }

        public async Task<IFileReference[]> ListAsync(string path, bool recursive)
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

            BlobContinuationToken continuationToken = null;
            List<IListBlobItem> results = new List<IListBlobItem>();
            do
            {
                var response = await this.container.Value.ListBlobsSegmentedAsync(path, recursive, BlobListingDetails.None, null, continuationToken, new BlobRequestOptions(), new OperationContext());
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);

            return results.OfType<ICloudBlob>().Select(blob => new Internal.AzureFileReference(blob)).ToArray();
        }

        public async Task<IFileReference[]> ListAsync(string path, string searchPattern, bool recursive)
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

            Microsoft.Extensions.FileSystemGlobbing.Matcher matcher = new Microsoft.Extensions.FileSystemGlobbing.Matcher(StringComparison.Ordinal);
            matcher.AddInclude(searchPattern);

            var operationContext = new OperationContext();
            BlobContinuationToken continuationToken = null;
            List<IListBlobItem> results = new List<IListBlobItem>();
            do
            {
                var response = await this.container.Value.ListBlobsSegmentedAsync(prefix, recursive, BlobListingDetails.None, null, continuationToken, new BlobRequestOptions(), new OperationContext());
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);

            var pathMap = results.OfType<ICloudBlob>().Select(blob => new Internal.AzureFileReference(blob)).ToDictionary(x => x.Path);

            var filteredResults = matcher.Execute(
                new Internal.AzureListDirectoryWrapper(path,
                pathMap));

            return filteredResults.Files.Select(x => pathMap[path + x.Path]).ToArray();
        }

        public async Task DeleteAsync(IPrivateFileReference file)
        {
            var fileReference = await InternalGetAsync(file);
            await fileReference.DeleteAsync();
        }
    }
}
