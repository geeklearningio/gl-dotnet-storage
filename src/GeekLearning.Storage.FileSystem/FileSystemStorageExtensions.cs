namespace GeekLearning.Storage
{
    using FileSystem;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class FileSystemStorageExtensions
    {
        public static IServiceCollection AddFileSystemStorage(this IServiceCollection services, string rootPath)
        {
            services.Configure<FileSystemOptions>(options => options.RootPath = rootPath);
            services.TryAddEnumerable(ServiceDescriptor.Transient<IStorageProvider, FileSystemStorageProvider>());
            return services;
        }
    }
}
