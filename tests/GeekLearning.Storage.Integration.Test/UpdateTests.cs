﻿namespace GeekLearning.Storage.Integration.Test
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

        [Theory(DisplayName = nameof(WriteAllText)), InlineData("azure"), InlineData("filesystem")]
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

        [Theory(DisplayName = nameof(SaveStream)), InlineData("azure"), InlineData("filesystem")]
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

        [Theory(DisplayName = nameof(AddMetatadaRoundtrip)), InlineData("azure")]
        public async Task AddMetatadaRoundtrip(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);

            var testFile = "Metadata/TextFile.txt";

            var file = await store.GetAsync(testFile);

            var id = Guid.NewGuid().ToString();

            await file.AddMetadataAsync(new Dictionary<string, string>
            {
                ["id"] = id
            });

            file = await store.GetAsync(testFile);

            var actualId = file.Metadata["id"];

            Assert.Equal(id, actualId);
        }

        [Theory(DisplayName = nameof(SaveMetatadaRoundtrip)), InlineData("azure")]
        public async Task SaveMetatadaRoundtrip(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);

            var testFile = "Metadata/TextFile.txt";

            var file = await store.GetAsync(testFile);

            var id = Guid.NewGuid().ToString();

            file.Metadata["id"] = id;

            await file.SaveMetadataAsync();

            file = await store.GetAsync(testFile);

            var actualId = file.Metadata["id"];

            Assert.Equal(id, actualId);
        }

        [Theory(DisplayName = nameof(ListMetatadaRoundtrip)), InlineData("azure")]
        public async Task ListMetatadaRoundtrip(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);

            var testFile = "Metadata/TextFile.txt";

            var file = await store.GetAsync(testFile);

            var id = Guid.NewGuid().ToString();

            file.Metadata["id"] = id;

            await file.SaveMetadataAsync();

            var files = await store.ListAsync("Metadata", withMetadata: true);

            string actualId = null;

            foreach (var aFile in files)
            {
                if (aFile.Path == testFile)
                {
                    actualId = aFile.Metadata["id"];
                }
            }

            Assert.Equal(id, actualId);
        }
    }
}
