namespace GeekLearning.Storage
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class StorageExtensions
    {
        public static IServiceCollection AddStorage(this IServiceCollection services)
        {
            services.TryAddTransient<IStorageFactory, Internal.StorageFactory>();
            return services;
        }
    }
}
