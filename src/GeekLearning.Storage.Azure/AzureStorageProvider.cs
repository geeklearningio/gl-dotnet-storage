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

        public IStore BuildStore(string storeName, IStorageStoreOptions storeOptions)
        {
            return new AzureStore(storeName, storeOptions.Parameters["ConnectionString"], storeOptions.Parameters["Container"]);
        }
    }
}
