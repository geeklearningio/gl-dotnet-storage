namespace GeekLearning.Storage.FileSystem.Internal
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class FileSystemFileReference : IFileReference
    {
        private readonly string storeName;
        private readonly Lazy<IFileProperties> propertiesLazy;
        private readonly Lazy<string> publicUrlLazy;
        private readonly IExtendedPropertiesProvider extendedPropertiesProvider;

        public FileSystemFileReference(
            string filePath,
            string path,
            string storeName,
            bool withMetadata,
            FileExtendedProperties extendedProperties,
            IPublicUrlProvider publicUrlProvider,
            IExtendedPropertiesProvider extendedPropertiesProvider)
        {
            this.FileSystemPath = filePath;
            this.Path = path.Replace('\\', '/');
            this.storeName = storeName;
            this.extendedPropertiesProvider = extendedPropertiesProvider;

            this.propertiesLazy = new Lazy<IFileProperties>(() =>
            {
                if (withMetadata)
                {
                    return new FileSystemFileProperties(this.FileSystemPath, extendedProperties);
                }

                throw new InvalidOperationException("Metadata are not loaded, please use withMetadata option");
            });

            this.publicUrlLazy = new Lazy<string>(() =>
            {
                if (publicUrlProvider != null)
                {
                    return publicUrlProvider.GetPublicUrl(storeName, this);
                }

                throw new InvalidOperationException("There is not FileSystemServer enabled.");
            });
        }

        public string FileSystemPath { get; }

        public string Path { get; }

        public string PublicUrl => this.publicUrlLazy.Value;

        public IFileProperties Properties => this.propertiesLazy.Value;

        public Task DeleteAsync()
        {
            File.Delete(this.FileSystemPath);
            return Task.FromResult(true);
        }

        public Task<string> GetExpirableUriAsync()
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadAllBytesAsync()
        {
            return Task.FromResult(File.ReadAllBytes(this.FileSystemPath));
        }

        public Task<string> ReadAllTextAsync()
        {
            return Task.FromResult(File.ReadAllText(this.FileSystemPath));
        }

        public Task<Stream> ReadAsync()
        {
            Stream stream = File.OpenRead(this.FileSystemPath);
            return Task.FromResult(stream);
        }

        public async Task ReadToStreamAsync(Stream targetStream)
        {
            using (var file = File.Open(this.FileSystemPath, FileMode.Open, FileAccess.Read))
            {
                await file.CopyToAsync(targetStream);
            }
        }

        public async Task UpdateAsync(Stream stream)
        {
            using (var file = File.Open(this.FileSystemPath, FileMode.Truncate, FileAccess.Write))
            {
                await stream.CopyToAsync(file);
            }
        }

        public Task SavePropertiesAsync()
        {
            if (this.extendedPropertiesProvider == null)
            {
                throw new InvalidOperationException("There is no FileSystem extended properties provider.");
            }

            return this.extendedPropertiesProvider.SaveExtendedPropertiesAsync(
                this.storeName,
                this,
                (this.Properties as FileSystemFileProperties).ExtendedProperties);
        }
    }
}
