﻿namespace GeekLearning.Storage.Integration.Test
{
    using Microsoft.Extensions.DependencyInjection;
    using Storage;
    using System.IO;
    using System.Threading.Tasks;
    using Xunit;

    [Collection(nameof(IntegrationCollection))]
    [Trait("Operation", "Read"), Trait("Kind", "Integration")]
    public class ReadTests
    {
        private StoresFixture storeFixture;

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

        [Theory(DisplayName = nameof(ReadAllBytesFromSubdirectoryFileUsingFileReference)), InlineData("azure"), InlineData("filesystem")]
        public async Task ReadAllBytesFromSubdirectoryFileUsingFileReference(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);

            var expectedText = ">42";

            var file = await store.GetAsync("SubDirectory/TextFile2.txt");

            using (var reader = new StreamReader(new MemoryStream(await file.ReadAllBytesAsync())))
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

        [Theory(DisplayName = nameof(ReadAllTextFromSubdirectoryFileUsingFileReference)), InlineData("azure"), InlineData("filesystem")]
        public async Task ReadAllTextFromSubdirectoryFileUsingFileReference(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);

            var expectedText = ">42";

            var file = await store.GetAsync("SubDirectory/TextFile2.txt");

            string actualText = await file.ReadAllTextAsync();

            Assert.Equal(expectedText, actualText);
        }


        [Theory(DisplayName = nameof(ListThenReadAllTextFromSubdirectoryFile)), InlineData("azure"), InlineData("filesystem")]
        public async Task ListThenReadAllTextFromSubdirectoryFile(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);

            var expectedText = ">42";

            var files = await store.ListAsync("SubDirectory");

            foreach (var file in files)
            {
                string actualText = await store.ReadAllTextAsync(file);

                Assert.Equal(expectedText, actualText);
            }
        }
    }
}
