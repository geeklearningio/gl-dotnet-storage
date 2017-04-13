namespace GeekLearning.Storage.Internal
{
    using System;
    using Configuration;
    using Microsoft.Extensions.Options;

    public abstract class StorageProviderBase<TParsedOptions, TInstanceOptions, TStoreOptions, TScopedStoreOptions> : IStorageProvider
        where TParsedOptions : class, IParsedOptions<TInstanceOptions, TStoreOptions, TScopedStoreOptions>, new()
        where TInstanceOptions : class, IProviderInstanceOptions, new()
        where TStoreOptions : class, IStoreOptions, new()
        where TScopedStoreOptions : class, TStoreOptions, IScopedStoreOptions
    {
        protected readonly TParsedOptions options;

        public StorageProviderBase(IOptions<TParsedOptions> options)
        {
            this.options = options.Value;
        }

        public abstract string Name { get; }

        public IStore BuildStore(string storeName)
        {
            return this.BuildStoreInternal(storeName, this.options.GetStoreConfiguration(storeName));
        }

        public IStore BuildStore(string storeName, IStoreOptions storeOptions)
        {
            if (storeOptions.ProviderType != this.Name)
            {
                throw new Exceptions.BadStoreProviderException(this.Name, storeName);
            }
           
            return this.BuildStoreInternal(
                storeName, 
                storeOptions.ParseStoreOptions<TParsedOptions, TInstanceOptions, TStoreOptions, TScopedStoreOptions>(options));
        }

        public IStore BuildScopedStore(string storeName, params object[] args)
        {
            var scopedStoreOptions = this.options.GetScopedStoreConfiguration(storeName);

            try
            {
                scopedStoreOptions.FolderName = string.Format(scopedStoreOptions.FolderNameFormat, args);
            }
            catch (Exception ex)
            {
                throw new Exceptions.BadScopedStoreConfiguration(storeName, "Cannot format folder name. See InnerException for details.", ex);
            }

            return this.BuildStoreInternal(storeName, scopedStoreOptions.ParseStoreOptions<TParsedOptions, TInstanceOptions, TStoreOptions, TScopedStoreOptions>(options));
        }

        protected abstract IStore BuildStoreInternal(string storeName, TStoreOptions storeOptions);
    }
}
