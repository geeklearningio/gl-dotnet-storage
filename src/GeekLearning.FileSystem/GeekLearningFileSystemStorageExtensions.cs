using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using GeekLearning.Storage;
using GeekLearning.Storage.FileSystem;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GeekLearningFileSystemStorageExtensions
    {

        public static IServiceCollection AddFileSystemStorage(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IStorageProvider, FileSystemStorageProvider>());
            return services;
        }
    }
}
