﻿namespace GeekLearning.Storage.Integration.Test
{
    using GeekLearning.Storage.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.PlatformAbstractions;
    using Microsoft.WindowsAzure.Storage;
    using Storage;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using GeekLearning.Storage.Azure.Configuration;
    using GeekLearning.Storage.FileSystem.Configuration;
    using System.Runtime.InteropServices;

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

        public string FileSystemSecondaryRootPath => Path.Combine(this.BasePath, "FileVault2");

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

            if (Directory.Exists(this.FileSystemSecondaryRootPath))
            {
                Directory.Delete(this.FileSystemSecondaryRootPath, true);
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var process = Process.Start(new ProcessStartInfo("cp")
                {
                    Arguments = $"-apv \"{Path.Combine(this.BasePath, "SampleDirectory")}\" \"{absolutePath}\""
                });

                if (!process.WaitForExit(30000))
                {
                    process.Kill();
                    throw new TimeoutException($"FileSystem Store '{storeName}' was not reset properly.");
                }

                if (process.ExitCode != 0)
                {
                    throw new TimeoutException($"FileSystem Store '{storeName}' was not copied properly.");
                }
            }
            else
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

        }

        private void ResetAzureStores()
        {
            var azCopy = Environment.ExpandEnvironmentVariables(Configuration["AzCopyPath"]);

            foreach (var parsedStoreKvp in this.AzureParsedOptions.ParsedStores)
            {
                var cloudStorageAccount = CloudStorageAccount.Parse(parsedStoreKvp.Value.ConnectionString);
                var cloudStoragekey = cloudStorageAccount.Credentials.ExportBase64EncodedKey();
                var containerName = parsedStoreKvp.Value.FolderName;

                var dest = cloudStorageAccount.BlobStorageUri.PrimaryUri.ToString() + containerName;

                var client = cloudStorageAccount.CreateCloudBlobClient();

                var container = client.GetContainerReference(containerName);
                container.CreateIfNotExistsAsync().Wait();

                var sas = container.GetSharedAccessSignature(new Microsoft.WindowsAzure.Storage.Blob.SharedAccessBlobPolicy
                {
                    SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(1),
                    Permissions = (Microsoft.WindowsAzure.Storage.Blob.SharedAccessBlobPermissions)(-1),
                });

                var arguments = $"copy \"{Path.Combine(this.BasePath, "SampleDirectory/*")}\" \"{dest}{sas}\"  --recursive=true";


                var processStartInfo = new ProcessStartInfo(azCopy)
                {
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                var process = new Process { StartInfo = processStartInfo };

                process.Start();

                if (!process.WaitForExit(30000))
                {
                    process.Kill();
                    throw new TimeoutException($"Azure Store '{parsedStoreKvp.Key}' was not reset properly.");
                }

                if (process.ExitCode != 0)
                {
                    var error =  process.StandardError.ReadToEnd();
                    throw new TimeoutException($"Azure Store '{parsedStoreKvp.Key}' was not populated because of an error: {error}");
                }
                
                var output = process.StandardOutput.ReadToEnd();
            }
        }
    }
}
