namespace GeekLearning.Storage
{
    public interface IStorageProvider
    {
        string Name { get; }

        IStore BuildStore(string storeName);

        IStore BuildStore(string storeName, IStorageStoreOptions storeOptions);
    }
}
