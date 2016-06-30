namespace GeekLearning.Storage
{
    public interface IStorageFactory
    {
        IStore GetStore(string store);
    }
}
