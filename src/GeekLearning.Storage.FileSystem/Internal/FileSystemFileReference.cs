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

        public FileSystemFileReference(string filePath, string path)
        {
            this.filePath = filePath;
            this.path = path.Replace('\\', '/');
        }

        public string FileSystemPath => this.filePath;

        public string Path => this.path;


        public string PublicUrl
        {
            get
            {
                throw new NotImplementedException();
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

        public async Task UpdateAsync(Stream stream)
        {
            using (var file = File.Open(this.filePath, FileMode.Truncate, FileAccess.Write))
            {
                await stream.CopyToAsync(file);
            }
        }
    }
}
