namespace GeekLearning.Storage.Configuration
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;

    public class StorageOptions : IParsedOptions<ProviderInstanceOptions, StoreOptions, ScopedStoreOptions>
    {
        internal const string GlobalOptionsName = "Global";

        private readonly Lazy<IReadOnlyDictionary<string, ProviderInstanceOptions>> parsedProviderInstances;
        private readonly Lazy<IReadOnlyDictionary<string, StoreOptions>> parsedStores;
        private readonly Lazy<IReadOnlyDictionary<string, ScopedStoreOptions>> parsedScopedStores;

        public StorageOptions()
        {
            this.parsedProviderInstances = new Lazy<IReadOnlyDictionary<string, ProviderInstanceOptions>>(
                () => this.Providers.Parse<ProviderInstanceOptions>());
            this.parsedStores = new Lazy<IReadOnlyDictionary<string, StoreOptions>>(
                () => this.Stores.Parse<StoreOptions>());
            this.parsedScopedStores = new Lazy<IReadOnlyDictionary<string, ScopedStoreOptions>>(
                () => this.ScopedStores.Parse<ScopedStoreOptions>());
        }

        public string Name => GlobalOptionsName;

        public Dictionary<string, IConfigurationSection> Providers { get; set; }

        public Dictionary<string, IConfigurationSection> Stores { get; set; }

        public Dictionary<string, IConfigurationSection> ScopedStores { get; set; }

        public IReadOnlyDictionary<string, ProviderInstanceOptions> ParsedProviderInstances { get => this.parsedProviderInstances.Value; set { } }

        public IReadOnlyDictionary<string, StoreOptions> ParsedStores { get => this.parsedStores.Value; set { } }

        public IReadOnlyDictionary<string, ScopedStoreOptions> ParsedScopedStores { get => this.parsedScopedStores.Value; set { } }

        public void BindStoreOptions(StoreOptions storeOptions, ProviderInstanceOptions providerInstanceOptions)
        {
        }
    }
}
