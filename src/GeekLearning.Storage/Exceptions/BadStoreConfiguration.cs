namespace GeekLearning.Storage.Exceptions
{
    using System;

    public class BadStoreConfiguration : Exception
    {
        public BadStoreConfiguration(string storeName) 
            : base($"The store '{storeName}' was not properly configured.")
        {
        }

        public BadStoreConfiguration(string storeName, string details)
            : base($"The store '{storeName}' was not properly configured. {details}")
        {
        }
    }
}
