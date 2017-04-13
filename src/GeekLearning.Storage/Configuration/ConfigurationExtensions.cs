namespace GeekLearning.Storage.Configuration
{
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;
    using System.Linq;

    public static class ConfigurationExtensions
    {
        public static IReadOnlyDictionary<string, TOptions> Parse<TOptions>(this IReadOnlyDictionary<string, IConfigurationSection> unparsedConfiguration)
            where TOptions : class, INamedElementOptions, new()
        {
            if (unparsedConfiguration == null)
            {
                return new Dictionary<string, TOptions>();
            }

            return unparsedConfiguration
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => BindOptions<TOptions>(kvp));
        }

        public static IStoreOptions GetStoreConfiguration<TInstanceOptions, TStoreOptions, TScopedStoreOptions>(this IParsedOptions<TInstanceOptions, TStoreOptions, TScopedStoreOptions> parsedOptions, string storeName, bool throwIfNotFound = true)
            where TInstanceOptions : class, IProviderInstanceOptions
            where TStoreOptions : class, IStoreOptions
            where TScopedStoreOptions : class, IScopedStoreOptions
        {
            parsedOptions.ParsedStores.TryGetValue(storeName, out var storeOptions);
            if (storeOptions != null)
            {
                return storeOptions;
            }

            parsedOptions.ParsedScopedStores.TryGetValue(storeName, out var scopedStoreOptions);
            if (scopedStoreOptions != null)
            {
                return scopedStoreOptions;
            }

            if (throwIfNotFound)
            {
                throw new Exceptions.StoreNotFoundException(storeName);
            }

            return null;
        }

        public static void Compute<TParsedOptions, TInstanceOptions, TStoreOptions, TScopedStoreOptions>(this TInstanceOptions parsedProviderInstance, TParsedOptions options)
            where TParsedOptions : class, IParsedOptions<TInstanceOptions, TStoreOptions, TScopedStoreOptions>
            where TInstanceOptions : class, IProviderInstanceOptions, new()
            where TStoreOptions : class, IStoreOptions, new()
            where TScopedStoreOptions : class, IScopedStoreOptions, new()
        {
            options.BindProviderInstanceOptions(parsedProviderInstance);
        }

        public static void Compute<TParsedOptions, TInstanceOptions, TStoreOptions, TScopedStoreOptions>(this TStoreOptions parsedStore, TParsedOptions options)
            where TParsedOptions : class, IParsedOptions<TInstanceOptions, TStoreOptions, TScopedStoreOptions>
            where TInstanceOptions : class, IProviderInstanceOptions, new()
            where TStoreOptions : class, IStoreOptions, new()
            where TScopedStoreOptions : class, IScopedStoreOptions, new()
        {
            if (string.IsNullOrEmpty(parsedStore.FolderName))
            {
                parsedStore.FolderName = parsedStore.Name;
            }

            TInstanceOptions instanceOptions = null;
            if (!string.IsNullOrEmpty(parsedStore.ProviderName))
            {
                options.ParsedProviderInstances.TryGetValue(parsedStore.ProviderName, out instanceOptions);
                if (instanceOptions == null)
                {
                    return;
                }

                parsedStore.ProviderType = instanceOptions.Type;
            }

            options.BindStoreOptions(parsedStore, instanceOptions);
        }

        public static TStoreOptions ParseStoreOptions<TParsedOptions, TInstanceOptions, TStoreOptions, TScopedStoreOptions>(this IStoreOptions storeOptions, TParsedOptions options)
            where TParsedOptions : class, IParsedOptions<TInstanceOptions, TStoreOptions, TScopedStoreOptions>, new()
            where TInstanceOptions : class, IProviderInstanceOptions, new()
            where TStoreOptions : class, IStoreOptions, new()
            where TScopedStoreOptions : class, IScopedStoreOptions, new()
        {
            if (!(storeOptions is TStoreOptions parsedStoreOptions))
            {
                parsedStoreOptions = new TStoreOptions
                {
                    Name = storeOptions.Name,
                    ProviderName = storeOptions.ProviderName,
                    ProviderType = storeOptions.ProviderType,
                    AccessLevel = storeOptions.AccessLevel,
                    FolderName = storeOptions.FolderName,
                };
            }

            parsedStoreOptions.Compute<TParsedOptions, TInstanceOptions, TStoreOptions, TScopedStoreOptions>(options);
            return parsedStoreOptions;
        }

        private static TOptions BindOptions<TOptions>(KeyValuePair<string, IConfigurationSection> kvp)
            where TOptions : class, INamedElementOptions, new()
        {
            var options = new TOptions
            {
                Name = kvp.Key,
            };

            ConfigurationBinder.Bind(kvp.Value, options);
            return options;
        }
    }
}
