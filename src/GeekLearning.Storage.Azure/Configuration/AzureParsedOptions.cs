namespace GeekLearning.Storage.Azure.Configuration
{
    using GeekLearning.Storage.Configuration;
    using System.Collections.Generic;

    public class AzureParsedOptions : IParsedOptions<AzureProviderInstanceOptions, AzureStoreOptions, AzureScopedStoreOptions>
    {
        public string Name => AzureStorageProvider.ProviderName;

        public IReadOnlyDictionary<string, string> ConnectionStrings { get; set; }

        public IReadOnlyDictionary<string, AzureProviderInstanceOptions> ParsedProviderInstances { get; set; }

        public IReadOnlyDictionary<string, AzureStoreOptions> ParsedStores { get; set; }

        public IReadOnlyDictionary<string, AzureScopedStoreOptions> ParsedScopedStores { get; set; }

        public void BindProviderInstanceOptions(AzureProviderInstanceOptions providerInstanceOptions)
        {
            if (!string.IsNullOrEmpty(providerInstanceOptions.ConnectionStringName)
                && string.IsNullOrEmpty(providerInstanceOptions.ConnectionString))
            {
                if (!this.ConnectionStrings.ContainsKey(providerInstanceOptions.ConnectionStringName))
                {
                    throw new Exceptions.BadProviderConfiguration(
                        providerInstanceOptions.Name,
                        $"The ConnectionString '{providerInstanceOptions.ConnectionStringName}' cannot be found. Did you call AddStorage with the ConfigurationRoot?");
                }

                providerInstanceOptions.ConnectionString = this.ConnectionStrings[providerInstanceOptions.ConnectionStringName];
            }
        }

        public void BindStoreOptions(AzureStoreOptions storeOptions, AzureProviderInstanceOptions providerInstanceOptions = null)
        {
            storeOptions.FolderName = storeOptions.FolderName.ToLowerInvariant();

            if (!string.IsNullOrEmpty(storeOptions.ConnectionStringName)
                && string.IsNullOrEmpty(storeOptions.ConnectionString))
            {
                if (!this.ConnectionStrings.ContainsKey(storeOptions.ConnectionStringName))
                {
                    throw new Exceptions.BadStoreConfiguration(
                        storeOptions.Name, 
                        $"The ConnectionString '{storeOptions.ConnectionStringName}' cannot be found. Did you call AddStorage with the ConfigurationRoot?");
                }

                storeOptions.ConnectionString = this.ConnectionStrings[storeOptions.ConnectionStringName];
            }

            if (providerInstanceOptions == null
                || storeOptions.ProviderName != providerInstanceOptions.Name)
            {
                return;
            }

            if (string.IsNullOrEmpty(storeOptions.ConnectionString))
            {
                storeOptions.ConnectionString = providerInstanceOptions.ConnectionString;
            }
            
            if (string.IsNullOrEmpty(storeOptions.AuthenticationMode))
            {
                storeOptions.AuthenticationMode = providerInstanceOptions.AuthenticationMode;
            }
        }
    }
}
