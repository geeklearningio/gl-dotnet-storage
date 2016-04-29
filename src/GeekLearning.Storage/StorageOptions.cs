using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Storage
{
    public class StorageOptions
    {
        public StorageOptions()
        {

        }

        public Dictionary<string, StorageStore> Stores { get; set; }

        public class StorageStore
        {
            public string Provider { get; set; }

            public Dictionary<string, string> Parameters { get; set; }
        }
    }
}
