using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using GeekLearning.Storage;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GeekLearningStorageExtensions
    {
        public static IServiceCollection AddStorage(this IServiceCollection services)
        {
            services.TryAddTransient<IStorageFactory, StorageFactory>();
            return services;
        }
    }
}
