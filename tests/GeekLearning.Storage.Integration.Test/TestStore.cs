namespace GeekLearning.Storage.Integration.Test
{
    using Microsoft.Extensions.Configuration;

    public class TestStore : IStorageStoreOptions
    {
        public string Provider { get; set; }

        public IConfigurationSection Parameters { get; set; }
    }
}
