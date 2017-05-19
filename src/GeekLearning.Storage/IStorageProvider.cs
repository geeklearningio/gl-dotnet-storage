namespace GeekLearning.Storage
{
    using Configuration;

    public interface IStorageProvider
    {
        string Name { get; }

        IStore BuildStore(string storeName);

        IStore BuildStore(string storeName, IStoreOptions storeOptions);

        IStore BuildScopedStore(string storeName, params object[] args);
    }
}
