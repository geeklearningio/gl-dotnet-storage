namespace GeekLearning.Storage.FileSystem.Internal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class FileSystemFileReference : IFileReference
    {
        private string filePath;
        private string path;
        private IPublicUrlProvider publicUrlProvider;
        private string storeName;
        private FileInfo fileInfo;

        public FileSystemFileReference(string filePath, string path, string storeName, IPublicUrlProvider publicUrlProvider)
        {
            this.storeName = storeName;
            this.publicUrlProvider = publicUrlProvider;
            this.filePath = filePath;
            this.path = path.Replace('\\', '/');
            this.fileInfo = new FileInfo(this.FileSystemPath);
        }

        public string FileSystemPath => this.filePath;

        public IDictionary<string, string> Metadata
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Path => this.path;


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

        public DateTimeOffset? LastModified => new DateTimeOffset(this.fileInfo.LastWriteTimeUtc, TimeZoneInfo.Local.BaseUtcOffset);

        public string ContentType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public long? Length => this.fileInfo.Length;

        public Task DeleteAsync()
        {
            File.Delete(this.filePath);
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
            Stream stream = File.OpenRead(this.filePath);
            return Task.FromResult(stream);
        }

        public async Task ReadToStreamAsync(Stream targetStream)
        {
            using (var file = File.Open(this.filePath, FileMode.Open, FileAccess.Read))
            {
                await file.CopyToAsync(targetStream);
            }
        }

        public async Task UpdateAsync(Stream stream)
        {
            using (var file = File.Open(this.filePath, FileMode.Truncate, FileAccess.Write))
            {
                await stream.CopyToAsync(file);
            }
        }

        public Task AddMetadataAsync(IDictionary<string, string> metadata)
        {
            throw new NotImplementedException();
        }

        public Task SaveMetadataAsync()
        {
            throw new NotImplementedException();
        }
    }
}
