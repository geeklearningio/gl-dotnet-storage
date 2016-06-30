namespace GeekLearning.Storage
{
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;
    using System.Linq;

    internal class StorageFactory : IStorageFactory
    {
        private IOptions<StorageOptions> options;
        private IEnumerable<IStorageProvider> storageProviders;

        public StorageFactory(IEnumerable<IStorageProvider> storageProviders, IOptions<StorageOptions> options)
        {
            this.storageProviders = storageProviders;
            this.options = options;
        }

        public IStore GetStore(string store)
        {
            var conf = this.options.Value.Stores[store];
            return this.storageProviders.FirstOrDefault(x => x.Name == conf.Provider).BuildStore(conf);
        }
    }
}
