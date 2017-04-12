namespace GeekLearning.Storage.Integration.Test
{
    using GeekLearning.Storage.Configuration;

    public class TestStore : IStoreOptions
    {
        public TestStore()
        {
            this.Name = "TestStore";
            this.ProviderType = "FileSystem";
        }

        public string ProviderName { get; set; }

        public string ProviderType { get; set; }

        public AccessLevel AccessLevel { get; set; }

        public string FolderName { get; set; }

        public string Name { get; set; }
    }
}
