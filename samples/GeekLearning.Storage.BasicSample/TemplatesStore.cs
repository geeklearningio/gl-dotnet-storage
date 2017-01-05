namespace GeekLearning.Storage.BasicSample
{
    public class TemplatesStore : StoreBase
    {
        public TemplatesStore(IStorageFactory storageFactory) : base("Templates", storageFactory)
        {
        }
    }
}
