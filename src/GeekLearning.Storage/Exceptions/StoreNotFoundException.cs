namespace GeekLearning.Storage.Exceptions
{
    using System;

    public class StoreNotFoundException : Exception
    {
        public StoreNotFoundException(string storeName)
            : base($"The configured store '{storeName}' was not found. Did you configure it properly in your appsettings.json?")
        {
        }
    }
}
