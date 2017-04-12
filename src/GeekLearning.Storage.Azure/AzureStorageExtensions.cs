namespace GeekLearning.Storage
{
    using Azure;
    using GeekLearning.Storage.Azure.Configuration;
    using GeekLearning.Storage.Configuration;
    using GeekLearning.Storage.Internal;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;

    public static class AzureStorageExtensions
    {
        public static IServiceCollection AddAzureStorage(this IServiceCollection services)
        {
            return services
                .AddSingleton<IConfigureOptions<AzureParsedOptions>, ConfigureProviderOptions<AzureParsedOptions, AzureProviderInstanceOptions, AzureStoreOptions, AzureScopedStoreOptions>>()
                .AddAzureStorageServices();
        }

        public static IServiceCollection AddAzureStorage(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<AzureParsedOptions>(configuration)
                .AddAzureStorageServices();
        }

        private static IServiceCollection AddAzureStorageServices(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IStorageProvider, AzureStorageProvider>());
            return services;
        }
    }
}
