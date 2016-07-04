namespace GeekLearning.Integration.Test
{
    using Microsoft.Extensions.DependencyInjection;
    using Storage;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using Microsoft.Extensions.PlatformAbstractions;
    using Microsoft.Extensions.Configuration;
    using System.Diagnostics;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class StoresFixture : IDisposable
    {
        private CloudStorageAccount cloudStorageAccount;
        private CloudBlobContainer container;

        public StoresFixture()
        {
            this.BasePath = PlatformServices.Default.Application.ApplicationBasePath;

            var builder = new ConfigurationBuilder()
                .SetBasePath(BasePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.development.json", optional: true).
                AddInMemoryCollection(new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("Storage:Stores:azure:Parameters:Container", Guid.NewGuid().ToString("N").ToLower())
                });

            this.Configuration = builder.Build();

            var services = new ServiceCollection();

            services.AddOptions();

            services.AddStorage()
                .AddAzureStorage()
                .AddFileSystemStorage(BasePath);

            services.Configure<StorageOptions>(Configuration.GetSection("Storage"));

            this.Services = services.BuildServiceProvider();

            ResetStores();

        }

        private void ResetStores()
        {
            ResetAzureStore();
            ResetFileSystemStore();
        }

        private void ResetFileSystemStore()
        {
            var directoryName = Configuration["Storage:Stores:filesystem:Parameters:Path"];
            var process = Process.Start(new ProcessStartInfo("robocopy.exe")
            {
                Arguments = $"\"{System.IO.Path.Combine(BasePath, "SampleDirectory")}\" \"{System.IO.Path.Combine(BasePath, directoryName)}\" /MIR"
            });

            if (process.WaitForExit(30000))
            {

            }
            else
            {
                throw new TimeoutException("File system store was not reset properly");
            }
        }

        private void ResetAzureStore()
        {
            var azCopy = System.IO.Path.Combine(
                Environment.ExpandEnvironmentVariables(Configuration["AzCopyPath"]),
                "AzCopy.exe");


            cloudStorageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(Configuration["Storage:Stores:azure:Parameters:ConnectionString"]);
            var key = cloudStorageAccount.Credentials.ExportBase64EncodedKey();
            var containerName = Configuration["Storage:Stores:azure:Parameters:Container"];
            var dest = cloudStorageAccount.BlobStorageUri.PrimaryUri.ToString() + containerName;

            var client = cloudStorageAccount.CreateCloudBlobClient();

            container = client.GetContainerReference(containerName);
            container.CreateAsync().Wait();

            var process = Process.Start(new ProcessStartInfo(azCopy)
            {
                Arguments = $"/Source:\"{System.IO.Path.Combine(BasePath, "SampleDirectory")}\" /Dest:\"{dest}\" /DestKey:{key} /S"
            });

            if (process.WaitForExit(30000))
            {

            }
            else
            {
                throw new TimeoutException("Azure store was not reset properly");
            }
        }

        public IConfigurationRoot Configuration { get; }
        public IServiceProvider Services { get; }
        public string BasePath { get; }

        public void Dispose()
        {
            container.DeleteIfExistsAsync().Wait();
        }
    }
}
