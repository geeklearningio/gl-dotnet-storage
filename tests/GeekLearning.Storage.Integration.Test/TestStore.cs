using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Storage.Integration.Test
{
    public class TestStore : IStorageStoreOptions
    {
        public string Provider { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
    }
}
