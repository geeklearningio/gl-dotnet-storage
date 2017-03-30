namespace GeekLearning.Storage
{
    using Azure;
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
                .AddSingleton<IConfigureOptions<ProviderOptions>, ConfigureProviderOptions<ProviderOptions, StoreOptions>>()
                .AddAzureStorageServices();
        }

        public static IServiceCollection AddAzureStorage(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<ProviderOptions>(configuration)
                .AddAzureStorageServices();
        }

        private static IServiceCollection AddAzureStorageServices(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IStorageProvider, AzureStorageProvider>());
            return services;
        }
    }
}
