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
    using System.Text;

    [Collection(nameof(IntegrationCollection))]
    [Trait("Operation", "Update"), Trait("Kind", "Integration")]
    public class UpdateTests
    {
        StoresFixture storeFixture;

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

            await store.SaveAsync(Encoding.UTF8.GetBytes(textToWrite), filePath , "text/plain");

            var readFromWrittenFile = await store.ReadAllTextAsync(filePath);

            Assert.Equal(textToWrite, readFromWrittenFile);
        }
    }
}
