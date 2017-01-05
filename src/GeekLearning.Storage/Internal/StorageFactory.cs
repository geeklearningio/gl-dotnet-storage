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
            return this.storageProviders.FirstOrDefault(x => x.Name == configuration.Provider).BuildStore(storeName, configuration);
        }

        public IStore GetStore(string storeName)
        {
            var conf = this.options.Value.Stores[storeName];
            return this.storageProviders.FirstOrDefault(x => x.Name == conf.Provider).BuildStore(storeName, conf);
        }

        public bool TryGetStore(string storeName, out IStore store)
        {
            StorageOptions.StorageStoreOptions conf;
            if (this.options.Value.Stores.TryGetValue(storeName, out conf))
            {
                store = this.storageProviders.FirstOrDefault(x => x.Name == conf.Provider).BuildStore(storeName, conf);
                return true;
            }

            store = null;
            return false;
        }

        public bool TryGetStore(string storeName, out IStore store, string provider)
        {
            StorageOptions.StorageStoreOptions conf;
            if (this.options.Value.Stores.TryGetValue(storeName, out conf))
            {
                if (provider == conf.Provider)
                {
                    store = this.storageProviders.FirstOrDefault(x => x.Name == conf.Provider).BuildStore(storeName, conf);
                    return true;
                }
            }

            store = null;
            return false;
        }
    }
}
