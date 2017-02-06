namespace GeekLearning.Storage.Integration.Test
{
    using Microsoft.Extensions.DependencyInjection;
    using Storage;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    [Collection(nameof(IntegrationCollection))]
    [Trait("Kind", "Integration")]
    public class GenericIStoreTests
    {
        private StoresFixture storeFixture;

        public GenericIStoreTests(StoresFixture fixture)
        {
            this.storeFixture = fixture;
        }

        [Fact]
        public async Task GenericListRootFiles()
        {
            var store = this.storeFixture.Services.GetRequiredService<IStore<TestStore>>();

            var expected = new string[] { "TextFile.txt", "template.hbs" };

            var results = await store.ListAsync(null);

            var missingFiles = expected.Except(results.Select(f => f.Path)).ToArray();

            var unexpectedFiles = results.Select(f => f.Path).Except(expected).ToArray();

            Assert.Empty(missingFiles);
            Assert.Empty(unexpectedFiles);
        }
    }
}
