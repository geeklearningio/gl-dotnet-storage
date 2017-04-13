namespace GeekLearning.Storage.Azure.Configuration
{
    using GeekLearning.Storage.Configuration;

    public class AzureScopedStoreOptions : AzureStoreOptions, IScopedStoreOptions
    {
        public string FolderNameFormat { get; set; }
    }
}
