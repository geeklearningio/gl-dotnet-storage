using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Storage
{
    public abstract class StoreBase
    {
        private readonly IStore store;

        public StoreBase(string storeName, IStorageFactory storageFactory)
        {
            this.store = storageFactory.GetStore(storeName);
        }

        public IStore Store { get { return this.store; } }
    }
}
