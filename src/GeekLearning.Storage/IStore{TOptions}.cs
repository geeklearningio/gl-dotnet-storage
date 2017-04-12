namespace GeekLearning.Storage
{
    using Configuration;

    public interface IStore<TOptions> : IStore
        where TOptions : class, IStoreOptions, new()
    {
    }
}
