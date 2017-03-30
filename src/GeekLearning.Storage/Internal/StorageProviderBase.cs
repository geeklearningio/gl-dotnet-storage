namespace GeekLearning.Storage.Internal
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;

    public abstract class StorageProviderBase<TProviderOptions, TStoreOptions> : IStorageProvider
        where TProviderOptions : class, IProviderOptions<TStoreOptions>, new()
        where TStoreOptions : class, IProviderStoreOptions, new()
    {
        protected readonly IOptions<TProviderOptions> options;

        public StorageProviderBase(IOptions<TProviderOptions> options)
        {
            this.options = options;
        }

        public abstract string Name { get; }

        public IStore BuildStore(string storeName)
        {
            if (this.options.Value.Stores.TryGetValue(storeName, out var storeOptions))
            {
                return this.BuildStore(storeName, storeOptions);
            }

            throw new Exceptions.BadStoreProviderException(this.Name, storeName);
        }

        public IStore BuildStore(string storeName, IStorageStoreOptions storageStoreOptions)
        {
            if (storageStoreOptions.Provider != this.Name)
            {
                throw new Exceptions.BadStoreProviderException(this.Name, storeName);
            }

            var storeOptions = new TStoreOptions();
            ConfigurationBinder.Bind(storageStoreOptions.Parameters, storeOptions);
            return this.BuildStore(storeName, storeOptions);
        }

        protected abstract IStore BuildStore(string storeName, TStoreOptions storeOptions);
    }
}
