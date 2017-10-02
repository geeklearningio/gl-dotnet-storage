namespace GeekLearning.Storage.Integration.Test
{
    using Microsoft.Extensions.DependencyInjection;
    using Storage;
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    [Collection(nameof(IntegrationCollection))]
    [Trait("Operation", "ScopedStores"), Trait("Kind", "Integration")]
    public class ScopedStoresTests
    {
        private StoresFixture storeFixture;

        public ScopedStoresTests(StoresFixture fixture)
        {
            this.storeFixture = fixture;
        }

        [Theory(DisplayName = nameof(ScopedStoreUpdate)), InlineData("ScopedStore1"), InlineData("ScopedStore2")]
        public async Task ScopedStoreUpdate(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var formatArg = Guid.NewGuid();
            var store = storageFactory.GetScopedStore(storeName, formatArg);

            await store.InitAsync();

            var textToWrite = "The answer is 42";
            var filePath = "Update/42.txt";

            await store.SaveAsync(Encoding.UTF8.GetBytes(textToWrite), filePath, "text/plain");

            var readFromWrittenFile = await store.ReadAllTextAsync(filePath);

            Assert.Equal(textToWrite, readFromWrittenFile);
        }
    }
}
