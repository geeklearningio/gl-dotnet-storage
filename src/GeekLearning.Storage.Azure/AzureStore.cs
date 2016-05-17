using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Storage.Azure
{
    public class AzureStore : IStore
    {
        private string connectionString;
        private Lazy<CloudBlobContainer> container;
        private string containerName;


        public AzureStore(string connectionString, string containerName)
        {
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

            container = new Lazy<CloudBlobContainer>(() => CloudStorageAccount.Parse(this.connectionString).CreateCloudBlobClient().GetContainerReference(this.containerName));
        }

        public Task<string> GetExpirableUri(string uri)
        {
            throw new NotImplementedException();
        }

        public async Task<MemoryStream> ReadInMemory(string path)
        {
            var memoryStream = new MemoryStream();
            var blockBlob = this.container.Value.GetBlockBlobReference(path);
            await blockBlob.DownloadToStreamAsync(memoryStream);
            return memoryStream;
        }

        public async Task<Stream> Read(string path)
        {
            return await this.ReadInMemory(path);
        }

        public async Task<byte[]> ReadAllBytes(string path)
        {
            var stream = await this.ReadInMemory(path);
            return stream.ToArray();
        }

        public async Task<string> ReadAllText(string path)
        {
            var blockBlob = this.container.Value.GetBlockBlobReference(path);
            return await blockBlob.DownloadTextAsync();
        }

        public async Task<string> Save(Stream data, string path, string mimeType)
        {
            var blockBlob = this.container.Value.GetBlockBlobReference(path);
            await blockBlob.UploadFromStreamAsync(data);
            blockBlob.Properties.ContentType = mimeType;
            blockBlob.Properties.CacheControl = "max-age=300, must-revalidate";
            await blockBlob.SetPropertiesAsync();
            return blockBlob.Uri.ToString();
        }

        public async Task<string> Save(byte[] data, string path, string mimeType)
        {
            var blockBlob = this.container.Value.GetBlockBlobReference(path);
            await blockBlob.UploadFromByteArrayAsync(data, 0, data.Length);
            blockBlob.Properties.ContentType = mimeType;
            blockBlob.Properties.CacheControl = "max-age=300, must-revalidate";
            await blockBlob.SetPropertiesAsync();
            return blockBlob.Uri.ToString();
        }

        public Task<string[]> List(string path)
        {
            return Task.FromResult(this.container.Value.ListBlobs(path).Select(blob => blob.Uri.ToString()).ToArray());
        }
    }
}
