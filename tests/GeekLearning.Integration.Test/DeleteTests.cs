namespace GeekLearning.Integration.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using GeekLearning.Storage;
    using Microsoft.Extensions.DependencyInjection;
    using System.IO;

    [Collection(nameof(IntegrationCollection))]
    [Trait("Operation", "Delete"), Trait("Kind", "Integration")]
    public class DeleteTests
    {
        StoresFixture storeFixture;

        public DeleteTests(StoresFixture fixture)
        {
            this.storeFixture = fixture;
        }

        [Theory(DisplayName = nameof(Delete)), InlineData("azure"), InlineData("filesystem")]
        public async Task Delete(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);

            var file = await store.GetAsync("Delete/ToDelete.txt");

            await file.DeleteAsync();

            Assert.Null(await store.GetAsync("Delete/ToDelete.txt"));
            Assert.NotNull(await store.GetAsync("Delete/ToSurvive.txt"));
        }
    }
}
