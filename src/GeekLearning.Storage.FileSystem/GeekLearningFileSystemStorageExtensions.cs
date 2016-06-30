namespace GeekLearning.Storage
{
    using FileSystem;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class GeekLearningFileSystemStorageExtensions
    {
        public static IServiceCollection AddFileSystemStorage(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IStorageProvider, FileSystemStorageProvider>());
            return services;
        }
    }
}
