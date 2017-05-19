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

    [Collection(nameof(IntegrationCollection))]
    [Trait("Operation", "Update"), Trait("Kind", "Integration")]
    public class UpdateTests
    {
        private StoresFixture storeFixture;

        public UpdateTests(StoresFixture fixture)
        {
            this.storeFixture = fixture;
        }

        [Theory(DisplayName = nameof(WriteAllText)), InlineData("Store1"), InlineData("Store2"), InlineData("Store3"), InlineData("Store4"), InlineData("Store5"), InlineData("Store6")]
        public async Task WriteAllText(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);
            var textToWrite = "The answer is 42";
            var filePath = "Update/42.txt";

            await store.SaveAsync(Encoding.UTF8.GetBytes(textToWrite), filePath, "text/plain");

            var readFromWrittenFile = await store.ReadAllTextAsync(filePath);

            Assert.Equal(textToWrite, readFromWrittenFile);
        }

        [Theory(DisplayName = nameof(ETagShouldBeTheSameWithSameContent)), InlineData("Store1"), InlineData("Store2"), InlineData("Store3"), InlineData("Store4"), InlineData("Store5"), InlineData("Store6")]
        public async Task ETagShouldBeTheSameWithSameContent(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);
            var textToWrite = "ETag Test Compute";
            var filePath = "Update/etag-same.txt";

            var savedReference = await store.SaveAsync(Encoding.UTF8.GetBytes(textToWrite), filePath, "text/plain");
            var readReference = await store.GetAsync(filePath, withMetadata: true);

            Assert.Equal(savedReference.Properties.ETag, readReference.Properties.ETag);
        }

        [Theory(DisplayName = nameof(ETagShouldBeDifferentWithDifferentContent)), InlineData("Store1"), InlineData("Store2"), InlineData("Store3"), InlineData("Store4"), InlineData("Store5"), InlineData("Store6")]
        public async Task ETagShouldBeDifferentWithDifferentContent(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);
            var textToWrite = "ETag Test Compute";
            var filePath = "Update/etag-different.txt";
            var textToUpdate = "ETag Test Compute 2";

            var savedReference = await store.SaveAsync(Encoding.UTF8.GetBytes(textToWrite), filePath, "text/plain");
            var updatedReference = await store.SaveAsync(Encoding.UTF8.GetBytes(textToUpdate), filePath, "text/plain");

            Assert.NotEqual(savedReference.Properties.ETag, updatedReference.Properties.ETag);
        }

        [Theory(DisplayName = nameof(SaveStream)), InlineData("Store1"), InlineData("Store2"), InlineData("Store3"), InlineData("Store4"), InlineData("Store5"), InlineData("Store6")]
        public async Task SaveStream(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);
            var textToWrite = "The answer is 42";
            var filePath = "Update/42-2.txt";

            await store.SaveAsync(new MemoryStream(Encoding.UTF8.GetBytes(textToWrite)), filePath, "text/plain");

            var readFromWrittenFile = await store.ReadAllTextAsync(filePath);

            Assert.Equal(textToWrite, readFromWrittenFile);
        }

        [Theory(DisplayName = nameof(AddMetatadaRoundtrip)), InlineData("Store1"), InlineData("Store2"), InlineData("Store3"), InlineData("Store4"), InlineData("Store5"), InlineData("Store6")]
        public async Task AddMetatadaRoundtrip(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);

            var testFile = "Metadata/TextFile.txt";

            var file = await store.GetAsync(testFile, withMetadata: true);

            var id = Guid.NewGuid().ToString();

            file.Properties.Metadata.Add("newid", id);

            await file.SavePropertiesAsync();

            file = await store.GetAsync(testFile, withMetadata: true);

            var actualId = file.Properties.Metadata["newid"];

            Assert.Equal(id, actualId);
        }

        [Theory(DisplayName = nameof(SaveMetatadaRoundtrip)), InlineData("Store1"), InlineData("Store2"), InlineData("Store3"), InlineData("Store4"), InlineData("Store5"), InlineData("Store6")]
        public async Task SaveMetatadaRoundtrip(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);

            var testFile = "Metadata/TextFile.txt";

            var file = await store.GetAsync(testFile, withMetadata: true);

            var id = Guid.NewGuid().ToString();

            file.Properties.Metadata["id"] = id;

            await file.SavePropertiesAsync();

            file = await store.GetAsync(testFile, withMetadata: true);

            var actualId = file.Properties.Metadata["id"];

            Assert.Equal(id, actualId);
        }

        [Theory(DisplayName = nameof(ListMetatadaRoundtrip)), InlineData("Store1"), InlineData("Store2"), InlineData("Store3"), InlineData("Store4"), InlineData("Store5"), InlineData("Store6")]
        public async Task ListMetatadaRoundtrip(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);

            var testFile = "Metadata/TextFile.txt";

            var file = await store.GetAsync(testFile, withMetadata: true);

            var id = Guid.NewGuid().ToString();

            file.Properties.Metadata["id"] = id;

            await file.SavePropertiesAsync();

            var files = await store.ListAsync("Metadata", withMetadata: true);

            string actualId = null;

            foreach (var aFile in files)
            {
                if (aFile.Path == testFile)
                {
                    actualId = aFile.Properties.Metadata["id"];
                }
            }

            Assert.Equal(id, actualId);
        }
    }
}
