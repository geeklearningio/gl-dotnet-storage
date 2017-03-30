namespace GeekLearning.Storage.FileSystem
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Storage;
    using System;

    public class FileSystemStorageProvider : IStorageProvider
    {
        private IOptions<FileSystemOptions> options;
        private IServiceProvider serviceProvider;

        public FileSystemStorageProvider(IOptions<FileSystemOptions> options, IServiceProvider serviceProvider)
        {
            this.options = options;
            this.serviceProvider = serviceProvider;
        }

        public string Name => "FileSystem";

        public IStore BuildStore(string storeName, IStorageStoreOptions storeOptions)
        {
            var publicUrlProvider = this.serviceProvider.GetService<IPublicUrlProvider>();
            var extendedPropertiesProvider = this.serviceProvider.GetService<IExtendedPropertiesProvider>();

            return new FileSystemStore(
                storeName,
                storeOptions.Parameters["Path"],
                this.options.Value.RootPath,
                publicUrlProvider,
                extendedPropertiesProvider);
        }
    }
}
