namespace GeekLearning.Storage
{
    public interface IStorageFactory
    {
        IStore GetStore(string storeName, IStorageStoreOptions configuration);

        IStore GetStore(string storeName);

        bool TryGetStore(string storeName, out IStore store);

        bool TryGetStore(string storeName, out IStore store, string provider);
    }
}
