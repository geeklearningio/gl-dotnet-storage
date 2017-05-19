namespace GeekLearning.Storage.FileSystem.Configuration
{
    using GeekLearning.Storage.Configuration;
    using System.Collections.Generic;
    using System.IO;

    public class FileSystemParsedOptions : IParsedOptions<FileSystemProviderInstanceOptions, FileSystemStoreOptions, FileSystemScopedStoreOptions>
    {
        public string Name => FileSystemStorageProvider.ProviderName;

        public IReadOnlyDictionary<string, string> ConnectionStrings { get; set; }

        public IReadOnlyDictionary<string, FileSystemProviderInstanceOptions> ParsedProviderInstances { get; set; }

        public IReadOnlyDictionary<string, FileSystemStoreOptions> ParsedStores { get; set; }

        public IReadOnlyDictionary<string, FileSystemScopedStoreOptions> ParsedScopedStores { get; set; }

        public string RootPath { get; set; }

        public void BindProviderInstanceOptions(FileSystemProviderInstanceOptions providerInstanceOptions)
        {
            if (string.IsNullOrEmpty(providerInstanceOptions.RootPath))
            {
                providerInstanceOptions.RootPath = this.RootPath;
            }
            else
            {
                if (!Path.IsPathRooted(providerInstanceOptions.RootPath))
                {
                    providerInstanceOptions.RootPath = Path.Combine(this.RootPath, providerInstanceOptions.RootPath);
                }
            }
        }

        public void BindStoreOptions(FileSystemStoreOptions storeOptions, FileSystemProviderInstanceOptions providerInstanceOptions = null)
        {
            if (string.IsNullOrEmpty(storeOptions.RootPath))
            {
                if (providerInstanceOptions != null
                    && storeOptions.ProviderName == providerInstanceOptions.Name)
                {
                    storeOptions.RootPath = providerInstanceOptions.RootPath;
                }
                else
                {
                    storeOptions.RootPath = this.RootPath;
                }
            }
        }
    }
}
