using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Storage.FileSystem
{
    public class FileSystemStore : IStore
    {
        private string absolutePath;
        public FileSystemStore(string path, string appPath)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }
            if (System.IO.Path.IsPathRooted(path))
            {
                this.absolutePath = path;
            }
            else
            {
                this.absolutePath = Path.Combine(appPath, path);
            }
        }

        public Task<string> GetExpirableUri(string uri)
        {
            return Task.FromResult(uri);
        }

        public Task<Stream> Read(string path)
        {
            return Task.FromResult((Stream)File.OpenRead(Path.Combine(this.absolutePath, path)));
        }

        public Task<byte[]> ReadAllBytes(string path)
        {
            return Task.FromResult(File.ReadAllBytes(Path.Combine(this.absolutePath, path)));
        }

        public Task<string> ReadAllText(string path)
        {
            return Task.FromResult(File.ReadAllText(Path.Combine(this.absolutePath, path)));
        }

        public Task<string> Save(Stream data, string path, string mimeType)
        {
            return Task.FromResult(File.ReadAllText(Path.Combine(this.absolutePath, path)));
        }

        public Task<string> Save(byte[] data, string path, string mimeType)
        {
            File.WriteAllBytes(Path.Combine(this.absolutePath, path), data);
            return Task.FromResult(path);
        }
    }
}
