namespace GeekLearning.Storage.Azure
{
    using GeekLearning.Storage.Internal;
    using Microsoft.Extensions.Options;
    using Storage;

    public class AzureStorageProvider : StorageProviderBase<ProviderOptions, StoreOptions>
    {
        public const string ProviderName = "Azure";

        public AzureStorageProvider(IOptions<ProviderOptions> options)
            : base(options)
        {
        }

        public override string Name => ProviderName;

        protected override IStore BuildStore(string storeName, StoreOptions storeOptions)
        {
            return new AzureStore(storeName, storeOptions.ConnectionString, storeOptions.Container);
        }
    }
}
