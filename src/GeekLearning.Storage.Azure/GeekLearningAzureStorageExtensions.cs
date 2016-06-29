namespace GeekLearning.Storage
{
    using Azure;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class GeekLearningAzureStorageExtensions
    {

        public static IServiceCollection AddAzureStorage(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IStorageProvider, AzureStorageProvider>());
            return services;
        }
    }
}
