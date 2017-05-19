namespace GeekLearning.Storage.FileSystem.ExtendedProperties.FileSystem.Internal
{
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Storage.FileSystem.Internal;
    using System.IO;
    using System.Threading.Tasks;

    public class ExtendedPropertiesProvider : IExtendedPropertiesProvider
    {
        private readonly FileSystemExtendedPropertiesOptions options;

        public ExtendedPropertiesProvider(
            IOptions<FileSystemExtendedPropertiesOptions> options)
        {
            this.options = options.Value;
        }

        public Task<FileExtendedProperties> GetExtendedPropertiesAsync(string storeAbsolutePath, IPrivateFileReference file)
        {
            var extendedPropertiesPath = this.GetExtendedPropertiesPath(storeAbsolutePath, file);
            if (!File.Exists(extendedPropertiesPath))
            {
                return Task.FromResult(new FileExtendedProperties());
            }

            var content = File.ReadAllText(extendedPropertiesPath);
            return Task.FromResult(JsonConvert.DeserializeObject<FileExtendedProperties>(content));
        }

        public Task SaveExtendedPropertiesAsync(string storeAbsolutePath, IPrivateFileReference file, FileExtendedProperties extendedProperties)
        {
            var extendedPropertiesPath = this.GetExtendedPropertiesPath(storeAbsolutePath, file);
            var toStore = JsonConvert.SerializeObject(extendedProperties);
            File.WriteAllText(extendedPropertiesPath, toStore);
            return Task.FromResult(0);
        }

        private string GetExtendedPropertiesPath(string storeAbsolutePath, IPrivateFileReference file)
        {
            var fullPath = Path.GetFullPath(storeAbsolutePath).TrimEnd(Path.DirectorySeparatorChar);
            var rootPath = Path.GetDirectoryName(fullPath);
            var storeName = Path.GetFileName(fullPath);

            var extendedPropertiesPath = Path.Combine(rootPath, string.Format(this.options.FolderNameFormat, storeName), file.Path + ".json");
            this.EnsurePathExists(extendedPropertiesPath);
            return extendedPropertiesPath;
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
