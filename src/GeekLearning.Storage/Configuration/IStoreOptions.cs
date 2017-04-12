namespace GeekLearning.Storage.Configuration
{
    public interface IStoreOptions : INamedElementOptions
    {
        string ProviderName { get; set; }

        string ProviderType { get; set; }

        AccessLevel AccessLevel { get; set; }

        string FolderName { get; set; }
    }
}
