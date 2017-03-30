namespace GeekLearning.Storage.Azure
{
    public class StoreOptions : IProviderStoreOptions
    {
        public string ConnectionString { get; set; }

        public string Container { get; set; }
    }
}
