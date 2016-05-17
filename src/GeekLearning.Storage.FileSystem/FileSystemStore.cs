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

        public Task<string[]> List(string path)
        {
            var directoryPath = Path.GetDirectoryName(Path.Combine(this.absolutePath, path));
            if (!Directory.Exists(directoryPath))
            {
                return Task.FromResult(new string[0]); 
            }
            return Task.FromResult(Directory.GetFiles(path).Select(x => x.Replace(this.absolutePath, "")).ToArray());
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
            EnsurePathExists(path);
            return Task.FromResult(File.ReadAllText(Path.Combine(this.absolutePath, path)));
        }

        public Task<string> Save(byte[] data, string path, string mimeType)
        {
            EnsurePathExists(path);
            File.WriteAllBytes(Path.Combine(this.absolutePath, path), data);
            return Task.FromResult(path);
        }

        private void EnsurePathExists(string path)
        {
            var directoryPath = Path.GetDirectoryName(Path.Combine(this.absolutePath, path));
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}
