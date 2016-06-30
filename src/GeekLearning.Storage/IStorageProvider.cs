namespace GeekLearning.Storage
{
    public interface IStorageProvider
    {
        string Name { get; }

        IStore BuildStore(StorageOptions.StorageStore storeOptions);
    }
}
