namespace GeekLearning.Storage.FileSystem.Internal
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class FileSystemFileReference : IFileReference
    {
        private readonly FileSystemStore store;
        private readonly Lazy<string> publicUrlLazy;
        private readonly IExtendedPropertiesProvider extendedPropertiesProvider;
        private bool withMetadata;
        private Lazy<IFileProperties> propertiesLazy;

        public FileSystemFileReference(
            string filePath,
            string path,
            FileSystemStore store,
            bool withMetadata,
            FileExtendedProperties extendedProperties,
            IPublicUrlProvider publicUrlProvider,
            IExtendedPropertiesProvider extendedPropertiesProvider)
        {
            this.FileSystemPath = filePath;
            this.Path = path.Replace('\\', '/');
            this.store = store;
            this.extendedPropertiesProvider = extendedPropertiesProvider;
            this.withMetadata = withMetadata;

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
                    return publicUrlProvider.GetPublicUrl(this.store.Name, this);
                }

                throw new InvalidOperationException("There is not FileSystemServer enabled.");
            });
        }

        public string FileSystemPath { get; }

        public string Path { get; }

        public string PublicUrl => this.publicUrlLazy.Value;

        public IFileProperties Properties => this.propertiesLazy.Value;

        public async Task DeleteAsync()
        {
            File.Delete(this.FileSystemPath);

            if (extendedPropertiesProvider != null)
            {
                await extendedPropertiesProvider.DeleteExtendedPropertiesAsync(this.FileSystemPath, this);
            }
        }

        public ValueTask<byte[]> ReadAllBytesAsync()
        {
            return new ValueTask<byte[]>(File.ReadAllBytes(this.FileSystemPath));
        }

        public ValueTask<string> ReadAllTextAsync()
        {
            return new ValueTask<string>(File.ReadAllText(this.FileSystemPath));
        }

        public ValueTask<Stream> ReadAsync()
        {
            return new ValueTask<Stream>(File.OpenRead(this.FileSystemPath));
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
                this.store.AbsolutePath,
                this,
                (this.Properties as FileSystemFileProperties).ExtendedProperties);
        }

        public ValueTask<string> GetSharedAccessSignature(ISharedAccessPolicy policy)
        {
            throw new NotSupportedException();
        }

        public async Task FetchProperties()
        {
            if (this.withMetadata)
            {
                return;
            }

            if (this.extendedPropertiesProvider == null)
            {
                throw new InvalidOperationException("There is no FileSystem extended properties provider.");
            }

            var extendedProperties = await this.extendedPropertiesProvider.GetExtendedPropertiesAsync(
                this.store.AbsolutePath,
                this);

            this.propertiesLazy = new Lazy<IFileProperties>(() => new FileSystemFileProperties(this.FileSystemPath, extendedProperties));
            this.withMetadata = true;
        }
    }
}
