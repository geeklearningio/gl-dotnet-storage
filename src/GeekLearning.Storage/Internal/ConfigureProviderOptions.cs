namespace GeekLearning.Storage.Internal
{
    using GeekLearning.Storage.Configuration;
    using Microsoft.Extensions.Options;
    using System.Linq;

    public class ConfigureProviderOptions<TParsedOptions, TInstanceOptions, TStoreOptions, TScopedStoreOptions> : IConfigureOptions<TParsedOptions>
        where TParsedOptions : class, IParsedOptions<TInstanceOptions, TStoreOptions, TScopedStoreOptions>
        where TInstanceOptions : class, IProviderInstanceOptions, new()
        where TStoreOptions : class, IStoreOptions, new()
        where TScopedStoreOptions : class, TStoreOptions, IScopedStoreOptions, new()
    {
        private readonly StorageOptions storageOptions;

        public ConfigureProviderOptions(IOptions<StorageOptions> storageOptions)
        {
            this.storageOptions = storageOptions.Value;
        }

        public void Configure(TParsedOptions options)
        {
            if (this.storageOptions == null)
            {
                return;
            }

            options.ConnectionStrings = this.storageOptions.ConnectionStrings;

            options.ParsedProviderInstances = this.storageOptions.Providers.Parse<TInstanceOptions>()
                .Where(kvp => kvp.Value.Type == options.Name)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            foreach (var parsedProviderInstance in options.ParsedProviderInstances)
            {
                parsedProviderInstance.Value.Compute<TParsedOptions, TInstanceOptions, TStoreOptions, TScopedStoreOptions>(options);
            }

            var parsedStores = this.storageOptions.Stores.Parse<TStoreOptions>();
            foreach (var parsedStore in parsedStores)
            {
                parsedStore.Value.Compute<TParsedOptions, TInstanceOptions, TStoreOptions, TScopedStoreOptions>(options);
            }

            options.ParsedStores = parsedStores
                .Where(kvp => kvp.Value.ProviderType == options.Name)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var parsedScopedStores = this.storageOptions.ScopedStores.Parse<TScopedStoreOptions>();
            foreach (var parsedScopedStore in parsedScopedStores)
            {
                parsedScopedStore.Value.Compute<TParsedOptions, TInstanceOptions, TStoreOptions, TScopedStoreOptions>(options);
            }

            options.ParsedScopedStores = parsedScopedStores
                .Where(kvp => kvp.Value.ProviderType == options.Name)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
