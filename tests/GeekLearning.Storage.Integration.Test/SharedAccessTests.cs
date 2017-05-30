namespace GeekLearning.Storage.Integration.Test
{
    using Storage;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.Extensions.Options;
    using GeekLearning.Storage.Azure.Configuration;
    using System.Linq;

    [Collection(nameof(IntegrationCollection))]
    [Trait("Operation", "SharedAccess"), Trait("Kind", "Integration")]
    public class SharedAccessTests
    {
        private readonly StoresFixture storeFixture;

        public SharedAccessTests(StoresFixture fixture)
        {
            this.storeFixture = fixture;
        }

        [Theory(DisplayName = nameof(StoreSharedAccess)), InlineData("Store3"), InlineData("Store4"), InlineData("Store5"), InlineData("Store6")]
        public async Task StoreSharedAccess(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();
            var options = this.storeFixture.Services.GetRequiredService<IOptions<AzureParsedOptions>>();

            var store = storageFactory.GetStore(storeName);

            options.Value.ParsedStores.TryGetValue(storeName, out var storeOptions);

            var sharedAccessSignature = await store.GetSharedAccessSignatureAsync(new SharedAccessPolicy
            {
                ExpiryTime = DateTime.UtcNow.AddHours(24),
                Permissions = SharedAccessPermissions.List,
            });

            var account = CloudStorageAccount.Parse(storeOptions.ConnectionString);            

            var accountSAS = new StorageCredentials(sharedAccessSignature);
            var accountWithSAS = new CloudStorageAccount(accountSAS, account.Credentials.AccountName, endpointSuffix: null, useHttps: true);
            var blobClientWithSAS = accountWithSAS.CreateCloudBlobClient();
            var containerWithSAS = blobClientWithSAS.GetContainerReference(storeOptions.FolderName);

            BlobContinuationToken continuationToken = null;
            List<IListBlobItem> results = new List<IListBlobItem>();

            do
            {
                var response = await containerWithSAS.ListBlobsSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);

            var filesFromStore = await store.ListAsync(null, false, false);

            Assert.Equal(filesFromStore.Length, results.OfType<ICloudBlob>().Count());
        }
    }
}
