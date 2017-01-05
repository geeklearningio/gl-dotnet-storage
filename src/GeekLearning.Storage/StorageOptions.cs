namespace GeekLearning.Storage
{
    using System.Collections.Generic;

    public class StorageOptions
    {
        public Dictionary<string, StorageStoreOptions> Stores { get; set; }

        public class StorageStoreOptions : IStorageStoreOptions
        {
            public string Provider { get; set; }

            public Dictionary<string, string> Parameters { get; set; }
        }
    }
}
