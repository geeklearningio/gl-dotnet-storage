namespace GeekLearning.Storage
{
    public abstract class StoreBase
    {
        private readonly IStore store;

        public StoreBase(string storeName, IStorageFactory storageFactory)
        {
            this.store = storageFactory.GetStore(storeName);
        }

        public IStore Store { get { return this.store; } }
    }
}
