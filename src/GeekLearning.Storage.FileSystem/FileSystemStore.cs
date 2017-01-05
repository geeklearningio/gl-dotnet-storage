namespace GeekLearning.Storage.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class FileSystemStore : IStore
    {
        private string absolutePath;
        private IPublicUrlProvider publicUrlProvider;

        public FileSystemStore(string storeName, string path, string rootPath, IPublicUrlProvider publicUrlProvider)
        {
            this.publicUrlProvider = publicUrlProvider;
            this.Name = storeName;

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

        public string Name { get; }

        private Internal.FileSystemFileReference InternalGetAsync(IPrivateFileReference file)
        {
            var reference = InternalGetOrCreateAsync(file);
            if (File.Exists(reference.FileSystemPath))
            {
                return reference;
            }

            return null;
        }

        private Internal.FileSystemFileReference InternalGetOrCreateAsync(IPrivateFileReference file)
        {
            var fullPath = Path.Combine(this.absolutePath, file.Path);
            return new Internal.FileSystemFileReference(fullPath, file.Path, this.Name, this.publicUrlProvider);
        }

        public Task<IFileReference> GetAsync(IPrivateFileReference file, bool withMetadata)
        {
            IFileReference fileReference = this.InternalGetAsync(file);
            return Task.FromResult(fileReference);
        }

        public Task<IFileReference> GetAsync(Uri uri, bool withMetadata)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(IPrivateFileReference file)
        {
            var fileReference = InternalGetAsync(file);
            return fileReference.DeleteAsync();
        }

        public Task<IFileReference[]> ListAsync(string path, bool recursive, bool withMetadata)
        {
            var directoryPath = (string.IsNullOrEmpty(path) || path == "/" || path == "\\") ? this.absolutePath : Path.Combine(this.absolutePath, path);
            if (!Directory.Exists(directoryPath))
            {
                return Task.FromResult(new IFileReference[0]);
            }

            return Task.FromResult(Directory.GetFiles(directoryPath)
                .Select(fullPath =>
                    (IFileReference)new Internal.FileSystemFileReference(fullPath, fullPath.Replace(this.absolutePath, "")
                    .Trim('/', '\\'), this.Name, this.publicUrlProvider))
                .ToArray());
        }

        public Task<IFileReference[]> ListAsync(string path, string searchPattern, bool recursive, bool withMetadata)
        {
            var directoryPath = (string.IsNullOrEmpty(path) || path == "/" || path == "\\") ? this.absolutePath : Path.Combine(this.absolutePath, path);
            if (!Directory.Exists(directoryPath))
            {
                return Task.FromResult(new IFileReference[0]);
            }

            Microsoft.Extensions.FileSystemGlobbing.Matcher matcher = new Microsoft.Extensions.FileSystemGlobbing.Matcher(StringComparison.Ordinal);
            matcher.AddInclude(searchPattern);

            var results = matcher.Execute(new Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoWrapper(new DirectoryInfo(directoryPath)));

            return Task.FromResult(results.Files
                .Select(match => (IFileReference)new Internal.FileSystemFileReference(Path.Combine(directoryPath, match.Path), Path.Combine(path, match.Path).Trim('/', '\\'), this.Name, this.publicUrlProvider))
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
            return fileReference.ReadAllBytesAsync();
        }

        public Task<string> ReadAllTextAsync(IPrivateFileReference file)
        {
            var fileReference = InternalGetAsync(file);
            return fileReference.ReadAllTextAsync();
        }

        public async Task<IFileReference> SaveAsync(Stream data, IPrivateFileReference file, string contentType)
        {
            var fileReference = InternalGetOrCreateAsync(file);
            EnsurePathExists(fileReference.FileSystemPath);
            using (var fileStream = File.Open(fileReference.FileSystemPath, FileMode.Create, FileAccess.Write))
            {
                await data.CopyToAsync(fileStream);
            }

            return fileReference;
        }

        public Task<IFileReference> SaveAsync(byte[] data, IPrivateFileReference file, string contentType)
        {
            var fileReference = InternalGetOrCreateAsync(file);
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

        public Task<IFileReference> AddMetadataAsync(IPrivateFileReference file, IDictionary<string, string> metadata)
        {
            throw new NotImplementedException();
        }
    }
}
