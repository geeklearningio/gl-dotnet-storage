using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace GeekLearning.Storage.BasicSample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            HostingEnvironement = env;
        }

        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment HostingEnvironement { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            var rng = RandomNumberGenerator.Create();
            byte[] signingKey = new byte[512];
            rng.GetBytes(signingKey);

            services.AddStorage()
                .AddAzureStorage()
                .AddFileSystemStorage(HostingEnvironement.ContentRootPath)
                .AddFileSystemStorageServer(options=> {
                    options.SigningKey = signingKey;
                    options.BaseUri = new Uri("http://localhost:11149/");
                });
            
            services.Configure<StorageOptions>(Configuration.GetSection("Storage"));

            services.AddScoped<TemplatesStore>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseFileSystemStorageServer();

            app.UseMvc();
        }
    }
}
