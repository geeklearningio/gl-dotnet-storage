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
    [Trait("Operation", "Read"), Trait("Kind", "Integration")]
    public class ReadTests
    {
        StoresFixture storeFixture;

        public ReadTests(StoresFixture fixture)
        {
            this.storeFixture = fixture;
        }

        [Theory(DisplayName = nameof(ReadAllTextFromRootFile)), InlineData("azure"), InlineData("filesystem")]
        public async Task ReadAllTextFromRootFile(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);

            var expectedText = "42";

            var actualText = await store.ReadAllTextAsync("TextFile.txt");

            Assert.Equal(expectedText, actualText);
        }

        [Theory(DisplayName = nameof(ReadAllTextFromRootFile)), InlineData("azure"), InlineData("filesystem")]
        public async Task ReadAllTextFromSubdirectoryFile(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);

            var expectedText = ">42";

            var actualText = await store.ReadAllTextAsync("SubDirectory/TextFile2.txt");

            Assert.Equal(expectedText, actualText);
        }

        [Theory(DisplayName = nameof(ReadAllBytesFromSubdirectoryFile)), InlineData("azure"), InlineData("filesystem")]
        public async Task ReadAllBytesFromSubdirectoryFile(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);

            var expectedText = ">42";

            using (var reader = new StreamReader(new MemoryStream(await store.ReadAllBytesAsync("SubDirectory/TextFile2.txt"))))
            {
                var actualText = reader.ReadToEnd();
                Assert.Equal(expectedText, actualText);
            }
        }


        [Theory(DisplayName = nameof(ReadFileFromSubdirectoryFile)), InlineData("azure"), InlineData("filesystem")]
        public async Task ReadFileFromSubdirectoryFile(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);

            var expectedText = ">42";

            var file = await store.GetAsync("SubDirectory/TextFile2.txt");

            string actualText = null;

            using (var reader = new StreamReader(await file.ReadAsync()))
            {
                actualText = await reader.ReadToEndAsync();
            }

            Assert.Equal(expectedText, actualText);
        }
    }
}
