namespace GeekLearning.Integration.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using GeekLearning.Storage;
    using Microsoft.Extensions.DependencyInjection;

    [Collection(nameof(Integration)), Trait("Operation", "List"), Trait("Kind", "Integration")]
    public class ListTests
    {
        StoresFixture storeFixture;

        public ListTests(StoresFixture fixture)
        {
            this.storeFixture = fixture;
        }

        [Theory(DisplayName = nameof(ListRootFiles)), InlineData("azure"), InlineData("filesystem")]
        public async Task ListRootFiles(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);

            var expected = new string[] { "TextFile.txt", "template.hbs" };

            var results = await store.ListAsync(null);

            var missingFiles = expected.Except(results.Select(f => f.Path)).ToArray();

            var unexpectedFiles = results.Select(f => f.Path).Except(expected).ToArray();

            Assert.Empty(missingFiles);
            Assert.Empty(unexpectedFiles);
        }

        [Theory(DisplayName = nameof(ListEmptyPathFiles)), InlineData("azure"), InlineData("filesystem")]
        public async Task ListEmptyPathFiles(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);

            var expected = new string[] { "TextFile.txt", "template.hbs" };

            var results = await store.ListAsync("");

            var missingFiles = expected.Except(results.Select(f => f.Path)).ToArray();

            var unexpectedFiles = results.Select(f => f.Path).Except(expected).ToArray();

            Assert.Empty(missingFiles);
            Assert.Empty(unexpectedFiles);
        }

        [Theory(DisplayName = nameof(ListSubDirectoryFiles)), InlineData("azure"), InlineData("filesystem")]
        public async Task ListSubDirectoryFiles(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);

            var expected = new string[] { "SubDirectory/TextFile2.txt" };

            var results = await store.ListAsync("SubDirectory");

            var missingFiles = expected.Except(results.Select(f => f.Path)).ToArray();

            var unexpectedFiles = results.Select(f => f.Path).Except(expected).ToArray();

            Assert.Empty(missingFiles);
            Assert.Empty(unexpectedFiles);
        }

        [Theory(DisplayName = nameof(ListSubDirectoryFilesWithTrailingSlash)), InlineData("azure"), InlineData("filesystem")]
        public async Task ListSubDirectoryFilesWithTrailingSlash(string storeName)
        {
            var storageFactory = this.storeFixture.Services.GetRequiredService<IStorageFactory>();

            var store = storageFactory.GetStore(storeName);

            var expected = new string[] { "SubDirectory/TextFile2.txt" };

            var results = await store.ListAsync("SubDirectory/");

            var missingFiles = expected.Except(results.Select(f => f.Path)).ToArray();

            var unexpectedFiles = results.Select(f => f.Path).Except(expected).ToArray();

            Assert.Empty(missingFiles);
            Assert.Empty(unexpectedFiles);
        }
    }
}
