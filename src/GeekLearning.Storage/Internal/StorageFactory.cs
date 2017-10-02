namespace GeekLearning.Storage.Internal
{
    using GeekLearning.Storage.Configuration;
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;
    using System.Linq;

    public class StorageFactory : IStorageFactory
    {
        private StorageOptions options;
        private IReadOnlyDictionary<string, IStorageProvider> storageProviders;

        public StorageFactory(IEnumerable<IStorageProvider> storageProviders, IOptions<StorageOptions> options)
        {
            this.storageProviders = storageProviders.ToDictionary(sp => sp.Name, sp => sp);
            this.options = options.Value;
        }

        public IStore GetStore(string storeName, IStoreOptions configuration)
        {
            return this.GetProvider(configuration).BuildStore(storeName, configuration);
        }

        public IStore GetStore(string storeName)
        {
            return this.GetProvider(this.options.GetStoreConfiguration(storeName)).BuildStore(storeName);
        }

        public IStore GetScopedStore(string storeName, params object[] args)
        {
            return this.GetProvider(this.options.GetScopedStoreConfiguration(storeName)).BuildScopedStore(storeName, args);
        }

        public bool TryGetStore(string storeName, out IStore store)
        {
            var configuration = this.options.GetStoreConfiguration(storeName, throwIfNotFound: false);
            if (configuration != null)
            {
                var provider = this.GetProvider(configuration, throwIfNotFound: false);
                if (provider != null)
                {
                    store = provider.BuildStore(storeName);
                    return true;
                }
            }

            store = null;
            return false;
        }

        public bool TryGetStore(string storeName, out IStore store, string providerName)
        {
            var configuration = this.options.GetStoreConfiguration(storeName, throwIfNotFound: false);
            if (configuration != null)
            {
                var provider = this.GetProvider(configuration, throwIfNotFound: false);
                if (provider != null && provider.Name == providerName)
                {
                    store = provider.BuildStore(storeName);
                    return true;
                }
            }

            store = null;
            return false;
        }

        private IStorageProvider GetProvider(IStoreOptions configuration, bool throwIfNotFound = true)
        {
            string providerTypeName = null;
            if (!string.IsNullOrEmpty(configuration.ProviderType))
            {
                providerTypeName = configuration.ProviderType;
            }
            else if (!string.IsNullOrEmpty(configuration.ProviderName))
            {
                this.options.ParsedProviderInstances.TryGetValue(configuration.ProviderName, out var providerInstanceOptions);
                if (providerInstanceOptions != null)
                {
                    providerTypeName = providerInstanceOptions.Type;
                }
                else if (throwIfNotFound)
                {
                    throw new Exceptions.BadProviderConfiguration(configuration.ProviderName, "Unable to find it in the configuration.");
                }
            }
            else if (throwIfNotFound)
            {
                throw new Exceptions.BadStoreConfiguration(configuration.Name, "You have to set either 'ProviderType' or 'ProviderName' on Store configuration.");
            }

            if (string.IsNullOrEmpty(providerTypeName))
            {
                return null;
            }

            this.storageProviders.TryGetValue(providerTypeName, out var provider);
            if (provider == null && throwIfNotFound)
            {
                throw new Exceptions.ProviderNotFoundException(providerTypeName);
            }

            return provider;
        }       
    }
}
