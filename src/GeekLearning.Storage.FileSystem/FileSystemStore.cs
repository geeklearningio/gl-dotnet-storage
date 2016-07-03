namespace GeekLearning.Storage.FileSystem
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class FileSystemStore : IStore
    {
        private string absolutePath;

        public FileSystemStore(string path, string rootPath)
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
                this.absolutePath = Path.Combine(rootPath, path);
            }
        }

        private Internal.FileSystemFileReference InternalGetAsync(IPrivateFileReference file)
        {
            return new Internal.FileSystemFileReference(Path.Combine(this.absolutePath, file.Path), file.Path);
        }

        public async Task<IFileReference> GetAsync(IPrivateFileReference file)
        {
            return InternalGetAsync(file);
        }

        public async Task<IFileReference> GetAsync(Uri uri)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(IPrivateFileReference file)
        {
            var fileReference = InternalGetAsync(file);
            return fileReference.DeleteAsync();
        }

        public Task<IFileReference[]> ListAsync(string path)
        {
            var directoryPath = Path.Combine(this.absolutePath, path);
            if (!Directory.Exists(directoryPath))
            {
                return Task.FromResult(new IFileReference[0]);
            }

            return Task.FromResult(Directory.GetFiles(directoryPath)
                .Select(fullPath => 
                    (IFileReference)new Internal.FileSystemFileReference(fullPath, fullPath.Replace(this.absolutePath, "")
                    .Trim('/', '\\')))
                .ToArray());
        }

        public Task<IFileReference[]> ListAsync(string path, string searchPattern)
        {
            var directoryPath = Path.Combine(this.absolutePath, path);
            if (!Directory.Exists(directoryPath))
            {
                return Task.FromResult(new IFileReference[0]);
            }

            return Task.FromResult(Directory.GetFiles(directoryPath, searchPattern)
                .Select(fullPath =>
                    (IFileReference)new Internal.FileSystemFileReference(fullPath, fullPath.Replace(this.absolutePath, "")
                    .Trim('/', '\\')))
                .ToArray());
        }

        public Task<Stream> ReadAsync(IPrivateFileReference file)
        {
            var fileReference = InternalGetAsync(file);
            return fileReference.ReadAsync();
        }

        public Task<byte[]> ReadAllBytesAsync(IPrivateFileReference file)
        {
            var fileReference = InternalGetAsync(file);
            return Task.FromResult(File.ReadAllBytes(fileReference.FileSystemPath));
        }

        public Task<string> ReadAllTextAsync(IPrivateFileReference file)
        {
            var fileReference = InternalGetAsync(file);
            return Task.FromResult(File.ReadAllText(fileReference.FileSystemPath));
        }

        public async Task<IFileReference> SaveAsync(Stream data, IPrivateFileReference file, string mimeType)
        {
            var fileReference = InternalGetAsync(file);
            EnsurePathExists(fileReference.FileSystemPath);
            using (var fileStream = File.Open(fileReference.FileSystemPath, FileMode.Create, FileAccess.Write))
            {
                await data.CopyToAsync(fileStream);
            }

            return fileReference;
        }

        public Task<IFileReference> SaveAsync(byte[] data, IPrivateFileReference file, string mimeType)
        {
            var fileReference = InternalGetAsync(file);
            EnsurePathExists(fileReference.FileSystemPath);
            File.WriteAllBytes(fileReference.FileSystemPath, data);
            return Task.FromResult((IFileReference)fileReference);
        }

        private void EnsurePathExists(string path)
        {
            var directoryPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}
