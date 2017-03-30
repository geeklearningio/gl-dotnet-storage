namespace GeekLearning.Storage
{
    using Microsoft.Extensions.Configuration;

    public interface IStorageStoreOptions
    {
        string Provider { get; }

        IConfigurationSection Parameters { get; }
    }
}
