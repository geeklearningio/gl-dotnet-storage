namespace GeekLearning.Storage.Configuration
{
    public class StoreOptions : IStoreOptions
    {
        public string Name { get; set; }

        public string ProviderName { get; set; }

        public string ProviderType { get; set; }

        public AccessLevel AccessLevel { get; set; }

        public string FolderName { get; set; }
    }
}
