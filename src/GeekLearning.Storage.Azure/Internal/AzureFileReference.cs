namespace GeekLearning.Storage.Azure.Internal
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class AzureFileReference : IFileReference
    {
        private Lazy<AzureFileProperties> propertiesLazy;
        private bool withMetadata;

        public AzureFileReference(string path, ICloudBlob cloudBlob, bool withMetadata)
        {
            this.Path = path;
            this.CloudBlob = cloudBlob;
            this.withMetadata = withMetadata;
            this.propertiesLazy = new Lazy<AzureFileProperties>(() =>
            {
                if (withMetadata && cloudBlob.Metadata != null && cloudBlob.Properties != null)
                {
                    return new AzureFileProperties(cloudBlob);
                }

                throw new InvalidOperationException("Metadata are not loaded, please use withMetadata option");
            });
        }

        public AzureFileReference(ICloudBlob cloudBlob, bool withMetadata) :
            this(cloudBlob.Name, cloudBlob, withMetadata)
        {
        }

        public string Path { get; }

        public IFileProperties Properties => this.propertiesLazy.Value;

        public string PublicUrl => this.CloudBlob.Uri.ToString();

        public ICloudBlob CloudBlob { get; }

        public Task DeleteAsync()
        {
            return this.CloudBlob.DeleteAsync();
        }

        public async ValueTask<Stream> ReadAsync()
        {
            return await this.ReadInMemoryAsync();
        }

        public async ValueTask<MemoryStream> ReadInMemoryAsync()
        {
            var memoryStream = new MemoryStream();
            await this.CloudBlob.DownloadRangeToStreamAsync(memoryStream, null, null);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        public Task UpdateAsync(Stream stream)
        {
            return this.CloudBlob.UploadFromStreamAsync(stream);
        }

        public async Task ReadToStreamAsync(Stream targetStream)
        {
            await this.CloudBlob.DownloadRangeToStreamAsync(targetStream, null, null);
        }

        public async ValueTask<string> ReadAllTextAsync()
        {
            using (var reader = new StreamReader(await this.CloudBlob.OpenReadAsync(AccessCondition.GenerateEmptyCondition(), new BlobRequestOptions(), new OperationContext())))
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
            var adHocPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessStartTime = policy.StartTime,
                SharedAccessExpiryTime = policy.ExpiryTime,
                Permissions = AzureStore.FromGenericToAzure(policy.Permissions),
            };

            return new ValueTask<string>(this.CloudBlob.GetSharedAccessSignature(adHocPolicy));
        }

        public async Task FetchProperties()
        {
            if (this.withMetadata)
            {
                return;
            }

            await this.CloudBlob.FetchAttributesAsync();

            this.propertiesLazy = new Lazy<AzureFileProperties>(() => new AzureFileProperties(this.CloudBlob));
            this.withMetadata = true;
        }
    }
}
