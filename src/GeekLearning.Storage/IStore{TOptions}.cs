namespace GeekLearning.Storage
{
    public interface IStore<TOptions> : IStore
        where TOptions : class, IStorageStoreOptions, new()
    {
    }
}
