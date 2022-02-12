using GeekLearning.Storage.Azure.Configuration;
using GeekLearning.Storage.Configuration;

namespace GeekLearning.Storage.Azure.Internal
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using global::Azure.Storage.Blobs;
    using global::Azure.Storage.Blobs.Models;
    using global::Azure.Storage.Sas;

    public class AzureFileReference : IFileReference
    {
        private Lazy<AzureFileProperties> propertiesLazy;
        private readonly BlobContainerClient client;
        private readonly StoreOptions storeOptions;
        private bool withMetadata;

        public AzureFileReference(BlobContainerClient client, AzureStoreOptions storeOptions, string path,
            BlobItem cloudBlob, bool withMetadata)
        {
            this.storeOptions = storeOptions;
            this.client = client;
            this.blobClient = client.GetBlobClient(path);
            this.Path = path;
            this.withMetadata = withMetadata;
            this.propertiesLazy = new Lazy<AzureFileProperties>(() =>
            {
                if (withMetadata && cloudBlob.Metadata != null && cloudBlob.Properties != null)
                {
                    return new AzureFileProperties(blobClient, cloudBlob);
                }

                throw new InvalidOperationException("Metadata are not loaded, please use withMetadata option");
            });
        }


        public AzureFileReference(BlobContainerClient client, string path, BlobProperties blobProperties)
        {
            this.client = client;
            this.blobClient = client.GetBlobClient(path);
            this.Path = path;

            this.withMetadata = true;
            this.propertiesLazy = new Lazy<AzureFileProperties>(() =>
            {
                return new AzureFileProperties(blobClient, blobProperties);
            });
        }


        public AzureFileReference(BlobContainerClient client, AzureStoreOptions options, BlobItem cloudBlob,
            bool withMetadata) :
            this(client, options, cloudBlob.Name, cloudBlob, withMetadata)
        {
        }

        public string Path { get; }

        private readonly BlobClient blobClient;

        public IFileProperties Properties => this.propertiesLazy.Value;

        public string PublicUrl => new Uri(this.client.Uri, $"${storeOptions.FolderName}/{Path}").ToString();

        public Task DeleteAsync()
        {
            return this.client.DeleteBlobAsync(Path);
        }

        public async ValueTask<Stream> ReadAsync()
        {
            return await this.ReadInMemoryAsync();
        }

        public async ValueTask<MemoryStream> ReadInMemoryAsync()
        {
            var memoryStream = new MemoryStream();
            var response = await blobClient.DownloadToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        public Task UpdateAsync(Stream stream)
        {
            return blobClient.UploadAsync(stream, overwrite: true);
        }

        public async Task ReadToStreamAsync(Stream targetStream)
        {
            await blobClient.DownloadToAsync(targetStream);
        }

        public async ValueTask<string> ReadAllTextAsync()
        {
            using (var reader = new StreamReader(await blobClient.OpenReadAsync()))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async ValueTask<byte[]> ReadAllBytesAsync()
        {
            return (await this.ReadInMemoryAsync()).ToArray();
        }

        public Task SavePropertiesAsync()
        {
            return this.propertiesLazy.Value.SaveAsync();
        }

        public ValueTask<string> GetSharedAccessSignature(ISharedAccessPolicy policy)
        {
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = client.Name,
                BlobName = blobClient.Name,
                Resource = "b",
            };

            if (policy.StartTime.HasValue)
            {
                sasBuilder.StartsOn = policy.StartTime.Value;
            }

            if (policy.ExpiryTime.HasValue)
            {
                sasBuilder.ExpiresOn = policy.ExpiryTime.Value;
            }

            sasBuilder.SetPermissions(AzureStore.FromGenericToAzure(policy.Permissions));

            return new ValueTask<string>(blobClient.GenerateSasUri(sasBuilder).Query);
        }

        public async Task FetchProperties()
        {
            if (this.withMetadata)
            {
                return;
            }

            var blobProperties = await this.blobClient.GetPropertiesAsync();

            this.propertiesLazy =
                new Lazy<AzureFileProperties>(() => new AzureFileProperties(this.blobClient, blobProperties));
            this.withMetadata = true;
        }
    }
}