namespace GeekLearning.Storage.Azure
{
    using GeekLearning.Storage.Azure.Configuration;
    using GeekLearning.Storage.Internal;
    using Microsoft.Extensions.Options;
    using Storage;

    public class AzureStorageProvider : StorageProviderBase<AzureParsedOptions, AzureProviderInstanceOptions, AzureStoreOptions, AzureScopedStoreOptions>
    {
        public const string ProviderName = "Azure";

        public AzureStorageProvider(IOptions<AzureParsedOptions> options)
            : base(options)
        {
        }

        public override string Name => ProviderName;

        protected override IStore BuildStoreInternal(string storeName, AzureStoreOptions storeOptions)
        {
            return new AzureStore(storeOptions);
        }
    }
}
