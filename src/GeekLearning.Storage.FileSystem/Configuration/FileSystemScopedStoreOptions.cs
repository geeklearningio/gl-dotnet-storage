namespace GeekLearning.Storage.FileSystem.Configuration
{
    using GeekLearning.Storage.Configuration;

    public class FileSystemScopedStoreOptions : FileSystemStoreOptions, IScopedStoreOptions
    {
        public string FolderNameFormat { get; set; }
    }
}
