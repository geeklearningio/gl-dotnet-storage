namespace GeekLearning.Storage.Integration.Test
{
    using GeekLearning.Storage.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.PlatformAbstractions;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Storage;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using GeekLearning.Storage.Azure.Configuration;
    using GeekLearning.Storage.FileSystem.Configuration;

    public class StoresFixture : IDisposable
    {
        public StoresFixture()
        {
            this.BasePath = PlatformServices.Default.Application.ApplicationBasePath;

            var containerId = Guid.NewGuid().ToString("N").ToLower();

            var builder = new ConfigurationBuilder()
                .SetBasePath(BasePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.development.json", optional: true)
                .AddInMemoryCollection(new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("Storage:Stores:Store3:FolderName", $"Store3-{containerId}"),
                    new KeyValuePair<string, string>("Storage:Stores:Store4:FolderName", $"Store4-{containerId}"),
                    new KeyValuePair<string, string>("Storage:Stores:Store5:FolderName", $"Store5-{containerId}"),
                    new KeyValuePair<string, string>("Storage:Stores:Store6:FolderName", $"Store6-{containerId}"),
                });

            this.Configuration = builder.Build();

            var services = new ServiceCollection();

            services.AddOptions();

            services.AddStorage(Configuration)
                .AddAzureStorage()
                .AddFileSystemStorage(this.FileSystemRootPath)
                .AddFileSystemExtendedProperties();

            services.Configure<StorageOptions>(Configuration.GetSection("Storage"));
            services.Configure<TestStore>(Configuration.GetSection("TestStore"));

            this.Services = services.BuildServiceProvider();
            this.StorageOptions = this.Services.GetService<IOptions<StorageOptions>>().Value;
            this.AzureParsedOptions = this.Services.GetService<IOptions<AzureParsedOptions>>().Value;
            this.FileSystemParsedOptions = this.Services.GetService<IOptions<FileSystemParsedOptions>>().Value;
            this.TestStoreOptions = this.Services.GetService<IOptions<TestStore>>().Value.ParseStoreOptions<FileSystemParsedOptions, FileSystemProviderInstanceOptions, FileSystemStoreOptions, FileSystemScopedStoreOptions>(this.FileSystemParsedOptions);
            ResetStores();
        }

        public IConfigurationRoot Configuration { get; }

        public IServiceProvider Services { get; }

        public string BasePath { get; }

        public string FileSystemRootPath => Path.Combine(this.BasePath, "FileVault");

        public StorageOptions StorageOptions { get; }

        public AzureParsedOptions AzureParsedOptions { get; }

        public FileSystemParsedOptions FileSystemParsedOptions { get; }

        public FileSystemStoreOptions TestStoreOptions { get; }

        public void Dispose()
        {
            this.DeleteRootResources();
        }

        private void DeleteRootResources()
        {
            foreach (var parsedStoreKvp in this.AzureParsedOptions.ParsedStores)
            {
                var cloudStorageAccount = CloudStorageAccount.Parse(parsedStoreKvp.Value.ConnectionString);
                var client = cloudStorageAccount.CreateCloudBlobClient();
                var container = client.GetContainerReference(parsedStoreKvp.Value.FolderName);

                container.DeleteIfExistsAsync().Wait();
            }

            if (Directory.Exists(this.FileSystemRootPath))
            {
                Directory.Delete(this.FileSystemRootPath, true);
            }
        }

        private void ResetStores()
        {
            this.DeleteRootResources();
            this.ResetAzureStores();
            this.ResetFileSystemStores();
        }

        private void ResetFileSystemStores()
        {
            if (!Directory.Exists(this.FileSystemRootPath))
            {
                Directory.CreateDirectory(this.FileSystemRootPath);
            }

            foreach (var parsedStoreKvp in this.FileSystemParsedOptions.ParsedStores)
            {
                ResetFileSystemStore(parsedStoreKvp.Key, parsedStoreKvp.Value.AbsolutePath);
            }

            ResetFileSystemStore(this.TestStoreOptions.Name, this.TestStoreOptions.AbsolutePath);
        }

        private void ResetFileSystemStore(string storeName, string absolutePath)
        {
            var process = Process.Start(new ProcessStartInfo("robocopy.exe")
            {
                Arguments = $"\"{Path.Combine(this.BasePath, "SampleDirectory")}\" \"{absolutePath}\" /MIR"
            });

            if (!process.WaitForExit(30000))
            {
                process.Kill();
                throw new TimeoutException($"FileSystem Store '{storeName}' was not reset properly.");
            }
        }

        private void ResetAzureStores()
        {
            var azCopy = Path.Combine(
                Environment.ExpandEnvironmentVariables(Configuration["AzCopyPath"]),
                "AzCopy.exe");

            foreach (var parsedStoreKvp in this.AzureParsedOptions.ParsedStores)
            {
                var cloudStorageAccount = CloudStorageAccount.Parse(parsedStoreKvp.Value.ConnectionString);
                var cloudStoragekey = cloudStorageAccount.Credentials.ExportBase64EncodedKey();
                var containerName = parsedStoreKvp.Value.FolderName;

                var dest = cloudStorageAccount.BlobStorageUri.PrimaryUri.ToString() + containerName;

                var client = cloudStorageAccount.CreateCloudBlobClient();

                var container = client.GetContainerReference(containerName);
                container.CreateIfNotExistsAsync().Wait();

                var arguments = $"/Source:\"{Path.Combine(this.BasePath, "SampleDirectory")}\" /Dest:\"{dest}\" /DestKey:{cloudStoragekey} /S /y";
                var process = Process.Start(new ProcessStartInfo(azCopy)
                {
                    Arguments = arguments
                });

                if (!process.WaitForExit(30000))
                {
                    process.Kill();
                    throw new TimeoutException($"Azure Store '{parsedStoreKvp.Key}' was not reset properly.");
                }
            }
        }
    }
}
