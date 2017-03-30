namespace GeekLearning.Storage.Integration.Test
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.PlatformAbstractions;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Storage;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    public class StoresFixture : IDisposable
    {
        private CloudStorageAccount cloudStorageAccount;
        private CloudBlobContainer container;

        public StoresFixture()
        {
            this.BasePath = PlatformServices.Default.Application.ApplicationBasePath;

            var containerId = Guid.NewGuid().ToString("N").ToLower();

            var builder = new ConfigurationBuilder()
                .SetBasePath(BasePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.development.json", optional: true)
                .AddInMemoryCollection(new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("Storage:Stores:azure:Parameters:Container", containerId),
                    new KeyValuePair<string, string>("TestStore:Parameters:Container", containerId)
                });

            this.Configuration = builder.Build();

            var services = new ServiceCollection();

            services.AddOptions();

            services.AddStorage()
                .AddAzureStorage()
                .AddFileSystemStorage(this.FileSystemRootPath)
                .AddFileSystemExtendedProperties();

            services.Configure<StorageOptions>(Configuration.GetSection("Storage"));
            services.Configure<TestStore>(Configuration.GetSection("TestStore"));

            this.Services = services.BuildServiceProvider();

            ResetStores();
        }

        public IConfigurationRoot Configuration { get; }

        public IServiceProvider Services { get; }

        public string BasePath { get; }

        public string FileSystemRootPath => Path.Combine(this.BasePath, "FileVault");

        public void Dispose()
        {
            this.DeleteRootResources();
        }

        private void DeleteRootResources()
        {
            if (this.container != null)
            {
                this.container.DeleteIfExistsAsync().Wait();
            }

            if (Directory.Exists(this.FileSystemRootPath))
            {
                Directory.Delete(this.FileSystemRootPath, true);
            }
        }

        private void ResetStores()
        {
            this.DeleteRootResources();
            this.ResetAzureStore();
            this.ResetFileSystemStore();
        }

        private void ResetFileSystemStore()
        {
            if (!Directory.Exists(this.FileSystemRootPath))
            {
                Directory.CreateDirectory(this.FileSystemRootPath);
            }

            var directoryName = Configuration["Storage:Stores:filesystem:Parameters:Path"];
            var process = Process.Start(new ProcessStartInfo("robocopy.exe")
            {
                Arguments = $"\"{Path.Combine(this.BasePath, "SampleDirectory")}\" \"{Path.Combine(this.FileSystemRootPath, directoryName)}\" /MIR"
            });

            if (!process.WaitForExit(30000))
            {
                throw new TimeoutException("File system store was not reset properly");
            }
        }

        private void ResetAzureStore()
        {
            var azCopy = Path.Combine(
                Environment.ExpandEnvironmentVariables(Configuration["AzCopyPath"]),
                "AzCopy.exe");

            cloudStorageAccount = CloudStorageAccount.Parse(Configuration["Storage:Stores:azure:Parameters:ConnectionString"]);
            var key = cloudStorageAccount.Credentials.ExportBase64EncodedKey();
            var containerName = Configuration["Storage:Stores:azure:Parameters:Container"];
            var dest = cloudStorageAccount.BlobStorageUri.PrimaryUri.ToString() + containerName;

            var client = cloudStorageAccount.CreateCloudBlobClient();

            this.container = client.GetContainerReference(containerName);
            this.container.CreateAsync().Wait();

            var process = Process.Start(new ProcessStartInfo(azCopy)
            {
                Arguments = $"/Source:\"{Path.Combine(this.BasePath, "SampleDirectory")}\" /Dest:\"{dest}\" /DestKey:{key} /S"
            });

            if (!process.WaitForExit(30000))
            {
                throw new TimeoutException("Azure store was not reset properly");
            }
        }
    }
}
