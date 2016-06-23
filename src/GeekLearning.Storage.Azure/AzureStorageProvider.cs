using GeekLearning.Storage;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Storage.Azure
{
    public class AzureStorageProvider : IStorageProvider
    {
        private AzureStorageManagerOptions options;

        public string Name
        {
            get
            {
                return "Azure";
            }
        }

        public IStore BuildStore(StorageOptions.StorageStore storeOptions)
        {
            return new AzureStore(storeOptions.Parameters["ConnectionString"], storeOptions.Parameters["Container"]);
        }

        //public AzureStorageProvider(IOptions<AzureStorageManagerOptions> options)
        //{
        //    this.options = options.Value;
        //}

        //public async Task<string> StoreFile(byte[] data, string substore, string path, string mimeType)
        //{
        //    var subStoreConfig = this.options.SubStores[substore];
        //    var account = CloudStorageAccount.Parse(subStoreConfig.ConnectionString);
        //    var blobClient = account.CreateCloudBlobClient();
        //    var container = blobClient.GetContainerReference(subStoreConfig.Container);
        //    return await SaveToStorage(container, data, path, mimeType);
        //}

        //private static async Task<string> SaveToStorage(Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer container, byte[] data, string path, string mimeType)
        //{
        //    var blockBlob = container.GetBlockBlobReference(path);
        //    await blockBlob.UploadFromByteArrayAsync(data, 0, data.Length);
        //    blockBlob.Properties.ContentType = mimeType;
        //    blockBlob.Properties.CacheControl = "max-age=300, must-revalidate";
        //    await blockBlob.SetPropertiesAsync();
        //    return blockBlob.Uri.ToString();
        //}

        //public async Task<string> GetExpirableUri(string substore, string uri)
        //{
        //    Microsoft.WindowsAzure.Storage.Blob.SharedAccessBlobPolicy policy = new Microsoft.WindowsAzure.Storage.Blob.SharedAccessBlobPolicy()
        //    {
        //        Permissions = Microsoft.WindowsAzure.Storage.Blob.SharedAccessBlobPermissions.Read,
        //        SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(5),
        //    };
        //    var subStoreConfig = this.options.SubStores[substore];
        //    var account = CloudStorageAccount.Parse(subStoreConfig.ConnectionString);
        //    var blobClient = account.CreateCloudBlobClient();
        //    var blobRef = await blobClient.GetBlobReferenceFromServerAsync(new Uri(uri));

        //    var sas = blobRef.GetSharedAccessSignature(policy);

        //    return uri + sas;
        //}

        //public Task<Stream> ReadFile()
        //{
        //    throw new NotImplementedException();
        //}

        ////private static async Task<string> SaveToStorage(Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer container, MemoryStream ms, string name)
        ////{
        ////    var blockBlob = container.GetBlockBlobReference(name);
        ////    await blockBlob.UploadFromStreamAsync(ms);
        ////    blockBlob.Properties.ContentType = "image/jpeg";
        ////    blockBlob.Properties.CacheControl = "max-age=300, must-revalidate";
        ////    await blockBlob.SetPropertiesAsync();
        ////    return blockBlob.Uri.ToString();
        ////}
    }
}
