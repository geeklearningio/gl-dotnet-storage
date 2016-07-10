namespace GeekLearning.Storage
{
    using FileSystem;
    using GeekLearning.Storage.FileSystem.Server;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public static class FileSystemStorageServerExtensions
    {
        public static IServiceCollection AddFileSystemStorageServer(this IServiceCollection services, Action<FileSystemStorageServerOptions> configure)
        {
            services.Configure<FileSystemStorageServerOptions>(configure);
            services.AddTransient<IPublicUrlProvider, FileSystem.Server.Internal.PublicUrlProvider>();
            return services;
        }

        public static IApplicationBuilder UseFileSystemStorageServer(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<FileSystemStorageServerOptions>>();
            app.Map(options.Value.EndpointPath, storePipeline =>
            {
                storePipeline.UseMiddleware<FileSystemStorageServerMiddleware>();
            });

            return app;
        } 
    }
}
