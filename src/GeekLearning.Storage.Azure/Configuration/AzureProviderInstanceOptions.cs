namespace GeekLearning.Storage.Azure.Configuration
{
    using GeekLearning.Storage.Configuration;

    public class AzureProviderInstanceOptions : ProviderInstanceOptions
    {
        public string ConnectionString { get; set; }

        public string ConnectionStringName { get; set; }
        
        public string AuthenticationMode { get; set; }
    }
}
