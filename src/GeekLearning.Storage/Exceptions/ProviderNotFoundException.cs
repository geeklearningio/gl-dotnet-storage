namespace GeekLearning.Storage.Exceptions
{
    using System;

    public class ProviderNotFoundException : Exception
    {
        public ProviderNotFoundException(string providerName)
            : base($"The configured provider '{providerName}' was not found. Did you forget to register providers in your Startup.ConfigureServices?")
        {
        }
    }
}
