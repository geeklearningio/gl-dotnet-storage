namespace GeekLearning.Storage.Internal
{
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;
    using System.Linq;

    public class StorageFactory : IStorageFactory
    {
        private IOptions<StorageOptions> options;
        private IEnumerable<IStorageProvider> storageProviders;

        public StorageFactory(IEnumerable<IStorageProvider> storageProviders, IOptions<StorageOptions> options)
        {
            this.storageProviders = storageProviders;
            this.options = options;
        }

        public IStore GetStore(string storeName, IStorageStoreOptions configuration)
        {
            return this.GetProvider(configuration.Provider).BuildStore(storeName, configuration);
        }

        public IStore GetStore(string storeName)
        {
            return this.GetProvider(this.GetStoreConfiguration(storeName).Provider).BuildStore(storeName);
        }

        public bool TryGetStore(string storeName, out IStore store)
        {
            if (this.options.Value.Stores.TryGetValue(storeName, out var configuration))
            {
                store = this.GetProvider(configuration.Provider).BuildStore(storeName);
                return true;
            }

            store = null;
            return false;
        }

        public bool TryGetStore(string storeName, out IStore store, string provider)
        {
            if (this.options.Value.Stores.TryGetValue(storeName, out var configuration))
            {
                if (provider == configuration.Provider)
                {
                    store = this.GetProvider(configuration.Provider).BuildStore(storeName);
                    return true;
                }
            }

            store = null;
            return false;
        }

        private IStorageProvider GetProvider(string providerName)
        {
            var provider = this.storageProviders.FirstOrDefault(p => p.Name == providerName);
            if (provider == null)
            {
                throw new Exceptions.ProviderNotFoundException(providerName);
            }

            return provider;
        }

        private StorageOptions.StorageStoreOptions GetStoreConfiguration(string storeName)
        {
            if (this.options.Value.Stores.TryGetValue(storeName, out var configuration))
            {
                return configuration;
            }

            throw new Exceptions.StoreNotFoundException(storeName);
        }
    }
}
