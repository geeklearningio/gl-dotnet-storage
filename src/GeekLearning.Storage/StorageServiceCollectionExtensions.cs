namespace GeekLearning.Storage
{
    using Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class StorageServiceCollectionExtensions
    {
        public static IServiceCollection AddStorage(this IServiceCollection services)
        {
            services.TryAddTransient<IStorageFactory, Internal.StorageFactory>();
            services.TryAdd(ServiceDescriptor.Transient(typeof(IStore<>), typeof(Internal.GenericStoreProxy<>)));
            return services;
        }

        public static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<StorageOptions>(configuration)
                .AddStorage();
        }
    }
}
