namespace GeekLearning.Storage.FileSystem
{
    using GeekLearning.Storage.Internal;
    using Microsoft.Extensions.Options;
    using Storage;

    public class FileSystemStorageProvider : StorageProviderBase<ProviderOptions, StoreOptions>
    {
        public const string ProviderName = "FileSystem";
        private readonly IPublicUrlProvider publicUrlProvider;
        private readonly IExtendedPropertiesProvider extendedPropertiesProvider;

        public FileSystemStorageProvider(IOptions<ProviderOptions> options, IPublicUrlProvider publicUrlProvider = null, IExtendedPropertiesProvider extendedPropertiesProvider = null)
            : base(options)
        {
            this.publicUrlProvider = publicUrlProvider;
            this.extendedPropertiesProvider = extendedPropertiesProvider;
        }

        public override string Name => ProviderName;

        protected override IStore BuildStore(string storeName, StoreOptions storeOptions)
        {
            return new FileSystemStore(
                storeName,
                storeOptions.Path,
                this.options.Value.RootPath,
                publicUrlProvider,
                extendedPropertiesProvider);
        }
    }
}
