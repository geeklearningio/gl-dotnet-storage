namespace GeekLearning.Storage
{
    using FileSystem;
    using GeekLearning.Storage.Internal;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;

    public static class FileSystemStorageExtensions
    {
        public static IServiceCollection AddFileSystemStorage(this IServiceCollection services, string rootPath)
        {
            return services
                .Configure<ProviderOptions>(options => options.RootPath = rootPath)
                .AddFileSystemStorage();
        }

        public static IServiceCollection AddFileSystemStorage(this IServiceCollection services)
        {
            return services
                .AddSingleton<IConfigureOptions<ProviderOptions>, ConfigureProviderOptions<ProviderOptions, StoreOptions>>()
                .AddFileSystemStorageServices();
        }

        public static IServiceCollection AddFileSystemStorage(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<ProviderOptions>(configuration)
                .AddFileSystemStorageServices();
        }

        private static IServiceCollection AddFileSystemStorageServices(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IStorageProvider, FileSystemStorageProvider>());
            return services;
        }
    }
}
