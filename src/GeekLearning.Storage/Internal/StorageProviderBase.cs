namespace GeekLearning.Storage.Internal
{
    using Configuration;
    using Microsoft.Extensions.Options;

    public abstract class StorageProviderBase<TParsedOptions, TInstanceOptions, TStoreOptions, TScopedStoreOptions> : IStorageProvider
        where TParsedOptions : class, IParsedOptions<TInstanceOptions, TStoreOptions, TScopedStoreOptions>, new()
        where TInstanceOptions : class, IProviderInstanceOptions, new()
        where TStoreOptions : class, IStoreOptions, new()
        where TScopedStoreOptions : class, IScopedStoreOptions, new()
    {
        protected readonly TParsedOptions options;

        public StorageProviderBase(IOptions<TParsedOptions> options)
        {
            this.options = options.Value;
        }

        public abstract string Name { get; }

        public IStore BuildStore(string storeName)
        {
            return this.BuildStore(storeName, this.options.GetStoreConfiguration(storeName));
        }

        public IStore BuildStore(string storeName, IStoreOptions storeOptions)
        {
            if (storeOptions.ProviderType != this.Name)
            {
                throw new Exceptions.BadStoreProviderException(this.Name, storeName);
            }
           
            return this.BuildStore(
                storeName, 
                storeOptions.ParseStoreOptions<TParsedOptions, TInstanceOptions, TStoreOptions, TScopedStoreOptions>(options));
        }

        protected abstract IStore BuildStore(string storeName, TStoreOptions storeOptions);
    }
}
