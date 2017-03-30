namespace GeekLearning.Storage.Azure
{
    using System.Collections.Generic;

    public class ProviderOptions : IProviderOptions<StoreOptions>
    {
        public string ProviderName => AzureStorageProvider.ProviderName;

        public string DefaultConnectionString { get; set; }

        public IReadOnlyDictionary<string, StoreOptions> Stores { get; set; }
    }
}
