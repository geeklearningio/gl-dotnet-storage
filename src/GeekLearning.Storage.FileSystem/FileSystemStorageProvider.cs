namespace GeekLearning.Storage.FileSystem
{
    using Storage;
    using Microsoft.Extensions.Options;

    public class FileSystemStorageProvider : IStorageProvider
    {
        private IOptions<FileSystemOptions> options;

        public FileSystemStorageProvider(IOptions<FileSystemOptions> options)
        {
            this.options = options;
        }

        public string Name
        {
            get
            {
                return "FileSystem";
            }
        }

        public IStore BuildStore(string storeName, StorageOptions.StorageStore storeOptions)
        {
            return new FileSystemStore(storeName, storeOptions.Parameters["Path"], this.options.Value.RootPath);
        }
    }
}
