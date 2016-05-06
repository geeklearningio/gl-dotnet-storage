using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using GeekLearning.Storage;
using GeekLearning.Storage.Azure;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GeekLearningAzureStorageExtensions
    {

        public static IServiceCollection AddAzureStorage(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IStorageProvider, AzureStorageProvider>());
            return services;
        }
    }
}
