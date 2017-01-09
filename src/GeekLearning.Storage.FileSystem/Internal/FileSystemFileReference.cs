namespace GeekLearning.Storage.FileSystem.Internal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class FileSystemFileReference : IFileReference
    {
        private IPublicUrlProvider publicUrlProvider;
        private string storeName;
        private FileInfo fileInfo;
        private Lazy<IFileProperties> propertiesLazy;

        public FileSystemFileReference(
            string filePath, 
            string path, 
            string storeName, 
            IPublicUrlProvider publicUrlProvider,
            bool withMetadata = false)
        {
            this.storeName = storeName;
            this.publicUrlProvider = publicUrlProvider;
            this.FileSystemPath = filePath;
            this.Path = path.Replace('\\', '/');
            this.fileInfo = new FileInfo(this.FileSystemPath);

            this.propertiesLazy = new Lazy<IFileProperties>(() =>
            {
                if (withMetadata)
                {
                    return new FileSystemFileProperties(this.fileInfo);
                }

                throw new InvalidOperationException("Metadata are not loaded, please use withMetadata option");
            });
        }

        public string FileSystemPath { get; }

        public string Path { get; }

        public string PublicUrl
        {
            get
            {
                if (publicUrlProvider != null)
                {
                    return publicUrlProvider.GetPublicUrl(storeName, this);
                }

                throw new NotSupportedException("There is not FileSystemServer enabled.");
            }
        }

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
            throw new NotImplementedException();
        }
    }
}
