using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Storage.FileSystem.Internal
{
    public class FileSystemFileReference : IFileReference
    {
        private string filePath;
        private string path;
        private IPublicUrlProvider publicUrlProvider;

        public FileSystemFileReference(string filePath, string path, IPublicUrlProvider publicUrlProvider)
        {
            this.publicUrlProvider = publicUrlProvider;
            this.filePath = filePath;
            this.path = path.Replace('\\', '/');
        }

        public string FileSystemPath => this.filePath;

        public string Path => this.path;


        public string PublicUrl
        {
            get
            {
                if (publicUrlProvider != null)
                {
                    return publicUrlProvider.GetPublicUrl(this);
                }

                throw new NotSupportedException("There is not FileSystemServer enabled.");
            }
        }

        public Task DeleteAsync()
        {
            File.Delete(this.filePath);
            return Task.FromResult(true);
        }

        public Task<string> GetExpirableUriAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Stream> ReadAsync()
        {
            return File.OpenRead(this.filePath);
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
    }
}
