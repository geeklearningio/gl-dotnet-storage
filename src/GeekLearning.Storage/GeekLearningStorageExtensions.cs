namespace GeekLearning.Storage
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class GeekLearningStorageExtensions
    {
        public static IServiceCollection AddStorage(this IServiceCollection services)
        {
            services.TryAddTransient<IStorageFactory, StorageFactory>();
            return services;
        }
    }
}
