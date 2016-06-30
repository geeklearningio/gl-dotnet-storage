namespace GeekLearning.Storage.FileSystem
{
    using Storage;
    using Microsoft.AspNetCore.Hosting;

    public class FileSystemStorageProvider : IStorageProvider
    {
        private IHostingEnvironment appEnv;

        public FileSystemStorageProvider(IHostingEnvironment appEnv)
        {
            this.appEnv = appEnv;
        }

        public string Name
        {
            get
            {
                return "FileSystem";
            }
        }

        public IStore BuildStore(StorageOptions.StorageStore storeOptions)
        {
            return new FileSystemStore(storeOptions.Parameters["Path"], this.appEnv.ContentRootPath);
        }
    }
}
