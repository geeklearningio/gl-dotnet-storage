namespace GeekLearning.Storage.Exceptions
{
    using System;

    public class BadScopedStoreConfiguration : Exception
    {
        public BadScopedStoreConfiguration(string storeName) 
            : base($"The scoped store '{storeName}' was not properly configured.")
        {
        }

        public BadScopedStoreConfiguration(string storeName, string details)
            : base($"The scoped store '{storeName}' was not properly configured. {details}")
        {
        }

        public BadScopedStoreConfiguration(string storeName, string details, Exception innerException)
            : base($"The scoped store '{storeName}' was not properly configured. {details}", innerException)
        {
        }
    }
}
