namespace GeekLearning.Storage.Internal
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using System.Linq;

    public class ConfigureProviderOptions<TProviderOptions, TStoreOptions> : IConfigureOptions<TProviderOptions>
        where TProviderOptions : class, IProviderOptions<TStoreOptions>, new()
        where TStoreOptions: class, IProviderStoreOptions, new()
    {
        private readonly IOptions<StorageOptions> storageOptions;

        public ConfigureProviderOptions(IOptions<StorageOptions> storageOptions)
        {
            this.storageOptions = storageOptions;
        }

        public void Configure(TProviderOptions options)
        {
            var storageOptionsValue = this.storageOptions.Value;

            if (storageOptionsValue == null)
            {
                return;
            }

            if (storageOptionsValue.Providers != null
                && storageOptionsValue.Providers.TryGetValue(options.ProviderName, out var providerConfigurationSection))
            {
                ConfigurationBinder.Bind(providerConfigurationSection, options);
            }

            if (storageOptionsValue.Stores != null)
            {
                options.Stores = storageOptionsValue.Stores
                    .Where(skvp => skvp.Value.Provider == options.ProviderName)
                    .ToDictionary(
                        skvp => skvp.Key,
                        skvp =>
                        {
                            var storeOptions = new TStoreOptions();
                            ConfigurationBinder.Bind(skvp.Value.Parameters, storeOptions);
                            return storeOptions;
                        });
            }
        }
    }
}
