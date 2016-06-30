namespace GeekLearning.Storage
{
    using System.Collections.Generic;

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
