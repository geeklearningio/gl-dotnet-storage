namespace GeekLearning.Storage.BasicSample
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Security.Cryptography;

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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var rng = RandomNumberGenerator.Create();
            byte[] signingKey = new byte[512];
            rng.GetBytes(signingKey);

            services.AddStorage(this.Configuration.GetSection("Storage"))
                .AddAzureStorage()
                .AddFileSystemStorage(HostingEnvironement.ContentRootPath)
                .AddFileSystemStorageServer(options =>
                {
                    options.SigningKey = signingKey;
                    options.BaseUri = new Uri("http://localhost:11149/");
                });

            services.AddScoped<TemplatesStore>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseFileSystemStorageServer();

            app.UseMvc();
        }
    }
}
