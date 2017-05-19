namespace GeekLearning.Storage.Configuration
{
    public interface IScopedStoreOptions : IStoreOptions
    {
        string FolderNameFormat { get; }
    }
}
