namespace GeekLearning.Storage.Azure
{
    using System.Collections.Generic;

    public class AzureStorageManagerOptions
    {
        public Dictionary<string, SubStore> SubStores { get; set; }

        public class SubStore
        {
            public string Container { get; set; }

            public string ConnectionString { get; set; }
        }
    }
}
