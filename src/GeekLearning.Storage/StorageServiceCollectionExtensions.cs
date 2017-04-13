namespace GeekLearning.Storage
{
    using Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using System.Collections.Generic;
    using System.Linq;

    public static class StorageServiceCollectionExtensions
    {
        public static IServiceCollection AddStorage(this IServiceCollection services)
        {
            services.TryAddTransient<IStorageFactory, Internal.StorageFactory>();
            services.TryAdd(ServiceDescriptor.Transient(typeof(IStore<>), typeof(Internal.GenericStoreProxy<>)));
            return services;
        }

        public static IServiceCollection AddStorage(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            return services
                .Configure<StorageOptions>(configurationSection)
                .AddStorage();
        }

        public static IServiceCollection AddStorage(this IServiceCollection services, IConfigurationRoot configurationRoot)
        {
            return services
                .Configure<StorageOptions>(configurationRoot.GetSection(StorageOptions.DefaultConfigurationSectionName))
                .Configure<StorageOptions>(storageOptions =>
                {
                    var connectionStrings = new Dictionary<string, string>();
                    ConfigurationBinder.Bind(configurationRoot.GetSection("ConnectionStrings"), connectionStrings);

                    if (storageOptions.ConnectionStrings != null)
                    {
                        foreach (var existingConnectionString in storageOptions.ConnectionStrings)
                        {
                            connectionStrings[existingConnectionString.Key] = existingConnectionString.Value;
                        }
                    }

                    storageOptions.ConnectionStrings = connectionStrings;
                })
                .AddStorage();
        }
    }
}
