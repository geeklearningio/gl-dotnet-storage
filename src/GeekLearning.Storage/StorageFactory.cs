using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;

namespace GeekLearning.Storage
{
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
