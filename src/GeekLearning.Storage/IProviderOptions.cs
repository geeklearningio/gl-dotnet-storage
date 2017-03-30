namespace GeekLearning.Storage
{
    using System.Collections.Generic;

    public interface IProviderOptions<TStoreOptions>
        where TStoreOptions: class, IProviderStoreOptions
    {
        string ProviderName { get; }

        IReadOnlyDictionary<string, TStoreOptions> Stores { get; set; }
    }
}
