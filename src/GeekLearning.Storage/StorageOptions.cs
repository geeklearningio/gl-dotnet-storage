namespace GeekLearning.Storage
{
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;

    public class StorageOptions
    {
        public Dictionary<string, IConfigurationSection> Providers { get; set; }

        public Dictionary<string, StorageStoreOptions> Stores { get; set; }

        public class StorageStoreOptions : IStorageStoreOptions
        {
            public string Provider { get; set; }

            public IConfigurationSection Parameters { get; set; }
        }
    }
}
