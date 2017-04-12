namespace GeekLearning.Storage.Internal
{
    using Configuration;
    using Microsoft.Extensions.Options;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class GenericStoreProxy<TOptions> : IStore, IStore<TOptions>
        where TOptions : class, IStoreOptions, new()
    {
        private IStore innerStore;

        public GenericStoreProxy(IStorageFactory factory, IOptions<TOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options", "Unable to build generic Store. Did you forget to configure your options?");
            }

            this.innerStore = factory.GetStore(nameof(TOptions), options.Value);
        }

        public string Name => innerStore.Name;

        public Task DeleteAsync(IPrivateFileReference file) => innerStore.DeleteAsync(file);

        public Task<IFileReference> GetAsync(Uri file, bool withMetadata) => innerStore.GetAsync(file, withMetadata);

        public Task<IFileReference> GetAsync(IPrivateFileReference file, bool withMetadata) => innerStore.GetAsync(file, withMetadata);

        public Task<IFileReference[]> ListAsync(string path, bool recursive, bool withMetadata) => innerStore.ListAsync(path, recursive, withMetadata);

        public Task<IFileReference[]> ListAsync(string path, string searchPattern, bool recursive, bool withMetadata) => innerStore.ListAsync(path, searchPattern, recursive, withMetadata);

        public Task<byte[]> ReadAllBytesAsync(IPrivateFileReference file) => innerStore.ReadAllBytesAsync(file);

        public Task<string> ReadAllTextAsync(IPrivateFileReference file) => innerStore.ReadAllTextAsync(file);

        public Task<Stream> ReadAsync(IPrivateFileReference file) => innerStore.ReadAsync(file);

        public Task<IFileReference> SaveAsync(Stream data, IPrivateFileReference file, string contentType) => innerStore.SaveAsync(data, file, contentType);

        public Task<IFileReference> SaveAsync(byte[] data, IPrivateFileReference file, string contentType) => innerStore.SaveAsync(data, file, contentType);
    }
}
