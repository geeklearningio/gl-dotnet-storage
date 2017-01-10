namespace GeekLearning.Storage.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    public class FileSystemStore : IStore
    {
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
                this.AbsolutePath = path;
            }
            else
            {
                this.AbsolutePath = Path.Combine(rootPath, path);
            }

            this.Name = storeName;
            this.publicUrlProvider = publicUrlProvider;
            this.extendedPropertiesProvider = extendedPropertiesProvider;
        }

        public string Name { get; }

        internal string AbsolutePath { get; }

        public async Task<IFileReference[]> ListAsync(string path, bool recursive, bool withMetadata)
        {
            var directoryPath = (string.IsNullOrEmpty(path) || path == "/" || path == "\\") ? this.AbsolutePath : Path.Combine(this.AbsolutePath, path);

            var result = new List<IFileReference>();
            if (Directory.Exists(directoryPath))
            {
                var allResultPaths = Directory.GetFiles(directoryPath)
                    .Select(fp => fp.Replace(this.AbsolutePath, "").Trim('/', '\\'))
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
            var directoryPath = (string.IsNullOrEmpty(path) || path == "/" || path == "\\") ? this.AbsolutePath : Path.Combine(this.AbsolutePath, path);

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

            var properties = fileReference.Properties as Internal.FileSystemFileProperties;

            properties.ContentType = contentType;
            properties.ExtendedProperties.ETag = GenerateEtag(fileReference.FileSystemPath);

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
            var fullPath = Path.Combine(this.AbsolutePath, path);
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
                    this.AbsolutePath,
                    new Storage.Internal.PrivateFileReference(path));
            }

            return new Internal.FileSystemFileReference(
                fullPath,
                path,
                this,
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

        private static string GenerateEtag(string fileSystemPath)
        {
            var etag = string.Empty;
            using (var stream = File.Open(fileSystemPath, FileMode.Open, FileAccess.Read))
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(stream);
                string hex = BitConverter.ToString(hash);
                etag = hex.Replace("-", "");
            }

            return $"\"{etag}\"";
        }
    }
}
