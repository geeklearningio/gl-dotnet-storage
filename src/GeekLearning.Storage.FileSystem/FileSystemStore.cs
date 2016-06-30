namespace GeekLearning.Storage.FileSystem
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class FileSystemStore : IStore
    {
        private string absolutePath;

        public FileSystemStore(string path, string appPath)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            if (Path.IsPathRooted(path))
            {
                this.absolutePath = path;
            }
            else
            {
                this.absolutePath = Path.Combine(appPath, path);
            }
        }

        public Task Delete(string path)
        {
            File.Delete(Path.Combine(this.absolutePath, path));
            return Task.FromResult(true);
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

            return Task.FromResult(Directory.GetFiles(directoryPath, path)
                .Select(x => x.Replace(this.absolutePath, "")
                .Trim('/', '\\'))
                .ToArray());
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

        public async Task<string> Save(Stream data, string path, string mimeType)
        {
            EnsurePathExists(path);
            using (var file = File.Open(Path.Combine(this.absolutePath, path), FileMode.Create, FileAccess.Write))
            {
                await data.CopyToAsync(file);
            }

            return path;
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
