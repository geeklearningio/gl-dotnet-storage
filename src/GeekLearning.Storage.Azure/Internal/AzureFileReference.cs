namespace GeekLearning.Storage.Azure.Internal
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class AzureFileReference : IFileReference
    {
        private ICloudBlob cloudBlob;

        public AzureFileReference(IListBlobItem blobItem)
           : this(blobItem as ICloudBlob)
        {
        }

        public AzureFileReference(string path, IListBlobItem blobItem)
            : this(path, blobItem as ICloudBlob)
        {
        }

        public AzureFileReference(string path, ICloudBlob cloudBlob)
        {
            this.Path = path;
            this.cloudBlob = cloudBlob;
        }

        public AzureFileReference(ICloudBlob cloudBlob) :
            this(cloudBlob.Name, cloudBlob)
        {

        }

        public DateTimeOffset? LastModified => this.cloudBlob.Properties?.LastModified;

        public string ContentType => this.cloudBlob.Properties?.ContentType;

        public long? Length => this.cloudBlob.Properties?.Length;

        public string Path { get; }

        public string PublicUrl => cloudBlob.Uri.ToString();

        public ICloudBlob CloudBlob => this.cloudBlob;

        public IDictionary<string, string> Metadata
        {
            get
            {
                if (this.cloudBlob.Metadata == null)
                {
                    throw new InvalidOperationException("Metadata are not loaded, please use withMetadata option");
                }

                return this.cloudBlob.Metadata;
            }
        }

        public Task DeleteAsync()
        {
            return this.cloudBlob.DeleteAsync();
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
            return this.cloudBlob.UploadFromStreamAsync(stream);
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
            using (var reader = new StreamReader(await cloudBlob.OpenReadAsync(AccessCondition.GenerateEmptyCondition(), new BlobRequestOptions(), new OperationContext())))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<byte[]> ReadAllBytesAsync()
        {
            return (await this.ReadInMemoryAsync()).ToArray();
        }

        public async Task AddMetadataAsync(IDictionary<string, string> metadata)
        {
            foreach (var pair in metadata)
            {
                this.cloudBlob.Metadata[pair.Key] = pair.Value;
            }

            await this.cloudBlob.SetMetadataAsync();
        }

        public Task SaveMetadataAsync()
        {
            return this.cloudBlob.SetMetadataAsync();
        }
    }
}
