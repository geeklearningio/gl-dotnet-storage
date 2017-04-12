namespace GeekLearning.Storage.Configuration
{
    using System.Collections.Generic;

    public interface IParsedOptions<TInstanceOptions, TStoreOptions, TScopedStoreOptions>
        where TInstanceOptions : class, IProviderInstanceOptions
        where TStoreOptions : class, IStoreOptions
        where TScopedStoreOptions : class, IScopedStoreOptions
    {
        string Name { get; }

        IReadOnlyDictionary<string, TInstanceOptions> ParsedProviderInstances { get; set; }

        IReadOnlyDictionary<string, TStoreOptions> ParsedStores { get; set; }

        IReadOnlyDictionary<string, TScopedStoreOptions> ParsedScopedStores { get; set; }

        void BindStoreOptions(TStoreOptions storeOptions, TInstanceOptions providerInstanceOptions = null);
    }
}
