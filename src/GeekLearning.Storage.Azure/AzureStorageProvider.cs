namespace GeekLearning.Storage.Azure
{
    using Storage;

    public class AzureStorageProvider : IStorageProvider
    {
        public string Name
        {
            get
            {
                return "Azure";
            }
        }

        public IStore BuildStore(StorageOptions.StorageStore storeOptions)
        {
            return new AzureStore(storeOptions.Parameters["ConnectionString"], storeOptions.Parameters["Container"]);
        }
    }
}
