namespace GeekLearning.Storage.FileSystem
{
    using System.Collections.Generic;

    public class ProviderOptions : IProviderOptions<StoreOptions>
    {
        public string ProviderName => FileSystemStorageProvider.ProviderName;

        public string RootPath { get; set; }

        public IReadOnlyDictionary<string, StoreOptions> Stores { get; set; }
    }
}
