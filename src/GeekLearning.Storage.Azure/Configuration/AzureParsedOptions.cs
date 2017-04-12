namespace GeekLearning.Storage.Azure.Configuration
{
    using GeekLearning.Storage.Configuration;
    using System.Collections.Generic;

    public class AzureParsedOptions : IParsedOptions<AzureProviderInstanceOptions, AzureStoreOptions, AzureScopedStoreOptions>
    {
        public string Name => AzureStorageProvider.ProviderName;

        public IReadOnlyDictionary<string, AzureProviderInstanceOptions> ParsedProviderInstances { get; set; }

        public IReadOnlyDictionary<string, AzureStoreOptions> ParsedStores { get; set; }

        public IReadOnlyDictionary<string, AzureScopedStoreOptions> ParsedScopedStores { get; set; }

        public void BindStoreOptions(AzureStoreOptions storeOptions, AzureProviderInstanceOptions providerInstanceOptions = null)
        {
            storeOptions.FolderName = storeOptions.FolderName.ToLowerInvariant();

            if (providerInstanceOptions == null
                || storeOptions.ProviderName != providerInstanceOptions.Name)
            {
                return;
            }

            if (string.IsNullOrEmpty(storeOptions.ConnectionString))
            {
                storeOptions.ConnectionString = providerInstanceOptions.ConnectionString;
            }

            if (string.IsNullOrEmpty(storeOptions.ConnectionStringName))
            {
                storeOptions.ConnectionStringName = providerInstanceOptions.ConnectionStringName;
            }
        }
    }
}
