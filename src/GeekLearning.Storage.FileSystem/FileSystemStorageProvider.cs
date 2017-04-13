namespace GeekLearning.Storage.FileSystem
{
    using GeekLearning.Storage.FileSystem.Configuration;
    using GeekLearning.Storage.Internal;
    using Microsoft.Extensions.Options;
    using Storage;

    public class FileSystemStorageProvider : StorageProviderBase<FileSystemParsedOptions, FileSystemProviderInstanceOptions, FileSystemStoreOptions, FileSystemScopedStoreOptions>
    {
        public const string ProviderName = "FileSystem";
        private readonly IPublicUrlProvider publicUrlProvider;
        private readonly IExtendedPropertiesProvider extendedPropertiesProvider;

        public FileSystemStorageProvider(IOptions<FileSystemParsedOptions> options, IPublicUrlProvider publicUrlProvider = null, IExtendedPropertiesProvider extendedPropertiesProvider = null)
            : base(options)
        {
            this.publicUrlProvider = publicUrlProvider;
            this.extendedPropertiesProvider = extendedPropertiesProvider;
        }

        public override string Name => ProviderName;

        protected override IStore BuildStoreInternal(string storeName, FileSystemStoreOptions storeOptions)
        {
            return new FileSystemStore(
                storeOptions,
                publicUrlProvider,
                extendedPropertiesProvider);
        }
    }
}
