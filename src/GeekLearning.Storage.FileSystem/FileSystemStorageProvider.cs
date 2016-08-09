namespace GeekLearning.Storage.FileSystem
{
    using Storage;
    using Microsoft.Extensions.Options;
    using System;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.DependencyInjection;

    public class FileSystemStorageProvider : IStorageProvider
    {
        private IOptions<FileSystemOptions> options;
        private IServiceProvider serviceProvider;

        public FileSystemStorageProvider(IOptions<FileSystemOptions> options, IServiceProvider serviceProvider)
        {
            this.options = options;
            this.serviceProvider = serviceProvider;
        }

        public string Name
        {
            get
            {
                return "FileSystem";
            }
        }

        public IStore BuildStore(string storeName, IStorageStoreOptions storeOptions)
        {
            var publicUrlProvider = this.serviceProvider.GetService<IPublicUrlProvider>();
            return new FileSystemStore(storeName, storeOptions.Parameters["Path"], this.options.Value.RootPath, publicUrlProvider);
        }
    }
}
