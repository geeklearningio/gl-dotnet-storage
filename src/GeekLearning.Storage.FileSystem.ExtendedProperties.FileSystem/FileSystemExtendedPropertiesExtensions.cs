namespace GeekLearning.Storage
{
    using FileSystem;
    using FileSystem.ExtendedProperties.FileSystem;
    using FileSystem.ExtendedProperties.FileSystem.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static class FileSystemExtendedPropertiesExtensions
    {
        public static IServiceCollection AddFileSystemExtendedProperties(this IServiceCollection services, Action<FileSystemExtendedPropertiesOptions> configure)
        {
            services.Configure<FileSystemExtendedPropertiesOptions>(configure);
            services.AddTransient<IExtendedPropertiesProvider, ExtendedPropertiesProvider>();
            return services;
        }
    }
}
