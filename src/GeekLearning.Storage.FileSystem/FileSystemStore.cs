namespace GeekLearning.Storage.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class FileSystemStore : IStore
    {
        private readonly string absolutePath;
        private readonly IPublicUrlProvider publicUrlProvider;
        private readonly IExtendedPropertiesProvider extendedPropertiesProvider;

        public FileSystemStore(string storeName, string path, string rootPath, IPublicUrlProvider publicUrlProvider, IExtendedPropertiesProvider extendedPropertiesProvider)
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

            this.Name = storeName;
            this.publicUrlProvider = publicUrlProvider;
            this.extendedPropertiesProvider = extendedPropertiesProvider;
        }

        public string Name { get; }

        public async Task<IFileReference[]> ListAsync(string path, bool recursive, bool withMetadata)
        {
            var directoryPath = (string.IsNullOrEmpty(path) || path == "/" || path == "\\") ? this.absolutePath : Path.Combine(this.absolutePath, path);

            var result = new List<IFileReference>();
            if (Directory.Exists(directoryPath))
            {
                var allResultPaths = Directory.GetFiles(directoryPath)
                    .Select(fp => fp.Replace(this.absolutePath, "").Trim('/', '\\'))
                    .ToList();

                foreach (var resultPath in allResultPaths)
                {
                    result.Add(await this.InternalGetAsync(resultPath, withMetadata));
                }
            }

            return result.ToArray();
        }

        public async Task<IFileReference[]> ListAsync(string path, string searchPattern, bool recursive, bool withMetadata)
        {
            var directoryPath = (string.IsNullOrEmpty(path) || path == "/" || path == "\\") ? this.absolutePath : Path.Combine(this.absolutePath, path);

            var result = new List<IFileReference>();
            if (Directory.Exists(directoryPath))
            {
                var matcher = new Microsoft.Extensions.FileSystemGlobbing.Matcher(StringComparison.Ordinal);
                matcher.AddInclude(searchPattern);

                var matches = matcher.Execute(new Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoWrapper(new DirectoryInfo(directoryPath)));
                var allResultPaths = matches.Files
                    .Select(match => Path.Combine(path, match.Path).Trim('/', '\\'))
                    .ToList();

                foreach (var resultPath in allResultPaths)
                {
                    result.Add(await this.InternalGetAsync(resultPath, withMetadata));
                }
            }

            return result.ToArray();
        }

        public async Task<IFileReference> GetAsync(IPrivateFileReference file, bool withMetadata)
        {
            return await this.InternalGetAsync(file, withMetadata);
        }

        public async Task<IFileReference> GetAsync(Uri uri, bool withMetadata)
        {
            if (uri.IsAbsoluteUri)
            {
                throw new InvalidOperationException("Cannot resolve an absolute URI with a FileSystem store.");
            }

            return await this.InternalGetAsync(uri.ToString(), withMetadata);
        }

        public async Task DeleteAsync(IPrivateFileReference file)
        {
            var fileReference = await this.InternalGetAsync(file);
            await fileReference.DeleteAsync();
        }

        public async Task<Stream> ReadAsync(IPrivateFileReference file)
        {
            var fileReference = await this.InternalGetAsync(file);
            return await fileReference.ReadAsync();
        }

        public async Task<byte[]> ReadAllBytesAsync(IPrivateFileReference file)
        {
            var fileReference = await this.InternalGetAsync(file);
            return await fileReference.ReadAllBytesAsync();
        }

        public async Task<string> ReadAllTextAsync(IPrivateFileReference file)
        {
            var fileReference = await this.InternalGetAsync(file);
            return await fileReference.ReadAllTextAsync();
        }

        public async Task<IFileReference> SaveAsync(byte[] data, IPrivateFileReference file, string contentType)
        {
            using (var stream = new MemoryStream(data, 0, data.Length))
            {
                return await this.SaveAsync(stream, file, contentType);
            }
        }

        public async Task<IFileReference> SaveAsync(Stream data, IPrivateFileReference file, string contentType)
        {
            var fileReference = await this.InternalGetAsync(file, withMetadata: true, checkIfExists: false);
            this.EnsurePathExists(fileReference.FileSystemPath);

            using (var fileStream = File.Open(fileReference.FileSystemPath, FileMode.Create, FileAccess.Write))
            {
                await data.CopyToAsync(fileStream);
            }

            fileReference.Properties.ContentType = contentType;
            await fileReference.SavePropertiesAsync();

            return fileReference;
        }

        private async Task<Internal.FileSystemFileReference> InternalGetAsync(IPrivateFileReference file, bool withMetadata = false, bool checkIfExists = true)
        {
            var fileSystemFile = file as Internal.FileSystemFileReference;
            if (fileSystemFile != null)
            {
                return fileSystemFile;
            }

            return await this.InternalGetAsync(file.Path, withMetadata, checkIfExists);
        }

        private async Task<Internal.FileSystemFileReference> InternalGetAsync(string path, bool withMetadata, bool checkIfExists = true)
        {
            var fullPath = Path.Combine(this.absolutePath, path);
            if (checkIfExists && !File.Exists(fullPath))
            {
                return null;
            }

            Internal.FileExtendedProperties extendedProperties = null;
            if (withMetadata)
            {
                if (this.extendedPropertiesProvider == null)
                {
                    throw new InvalidOperationException("There is no FileSystem extended properties provider.");
                }

                extendedProperties = await this.extendedPropertiesProvider.GetExtendedPropertiesAsync(
                    this.Name,
                    new Storage.Internal.PrivateFileReference(path));
            }

            return new Internal.FileSystemFileReference(
                fullPath,
                path,
                this.Name,
                withMetadata,
                extendedProperties,
                this.publicUrlProvider,
                this.extendedPropertiesProvider);
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
