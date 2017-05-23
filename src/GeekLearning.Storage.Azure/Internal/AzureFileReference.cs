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

        public AzureFileReference(string path, ICloudBlob cloudBlob, bool withMetadata)
        {
            this.Path = path;
            this.CloudBlob = cloudBlob;
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

        public async Task<Stream> ReadAsync()
        {
            return await this.ReadInMemoryAsync();
        }

        public async Task<MemoryStream> ReadInMemoryAsync()
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

        public Task<string> GetExpirableUriAsync()
        {
            throw new NotImplementedException();
        }

        public async Task ReadToStreamAsync(Stream targetStream)
        {
            await this.CloudBlob.DownloadRangeToStreamAsync(targetStream, null, null);
        }

        public async Task<string> ReadAllTextAsync()
        {
            using (var reader = new StreamReader(await this.CloudBlob.OpenReadAsync(AccessCondition.GenerateEmptyCondition(), new BlobRequestOptions(), new OperationContext())))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<byte[]> ReadAllBytesAsync()
        {
            return (await this.ReadInMemoryAsync()).ToArray();
        }

        public Task SavePropertiesAsync()
        {
            return this.propertiesLazy.Value.SaveAsync();
        }
    }
}
