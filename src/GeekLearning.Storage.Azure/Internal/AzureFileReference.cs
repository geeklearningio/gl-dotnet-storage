using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Storage.Azure.Internal
{
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

        public string Path { get; }

        public string PublicUrl => cloudBlob.Uri.ToString();

        public ICloudBlob CloudBlob => this.cloudBlob;

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
    }
}
