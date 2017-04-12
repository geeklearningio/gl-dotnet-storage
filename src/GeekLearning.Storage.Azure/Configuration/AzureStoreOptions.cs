namespace GeekLearning.Storage.Azure.Configuration
{
    using GeekLearning.Storage.Configuration;

    public class AzureStoreOptions : StoreOptions
    {
        public string ConnectionString { get; set; }

        public string ConnectionStringName { get; set; }
    }
}
