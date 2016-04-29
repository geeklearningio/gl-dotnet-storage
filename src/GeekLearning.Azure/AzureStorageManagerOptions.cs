using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Storage.Azure
{
    public class AzureStorageManagerOptions
    {

        public Dictionary<string,SubStore> SubStores { get; set; }

        public class SubStore
        {
            public string Container { get; set; }

            public string ConnectionString { get; set; }
        }
    }
}
