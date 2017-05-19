namespace GeekLearning.Storage.Exceptions
{
    using System;

    public class BadStoreProviderException : Exception
    {
        public BadStoreProviderException(string providerName, string storeName) 
            : base($"The store '{storeName}' was not configured with the provider '{providerName}'. Unable to build it.")
        {
        }
    }
}
