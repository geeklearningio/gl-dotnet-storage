namespace GeekLearning.Storage.Integration.Test
{
    using System.Collections.Generic;

    public class TestStore : IStorageStoreOptions
    {
        public string Provider { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
    }
}
