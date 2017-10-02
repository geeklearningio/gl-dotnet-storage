namespace GeekLearning.Storage
{
    using Configuration;

    public interface IStorageFactory
    {
        IStore GetStore(string storeName, IStoreOptions configuration);

        IStore GetStore(string storeName);

        IStore GetScopedStore(string storeName, params object[] args);

        bool TryGetStore(string storeName, out IStore store);

        bool TryGetStore(string storeName, out IStore store, string provider);
    }
}
