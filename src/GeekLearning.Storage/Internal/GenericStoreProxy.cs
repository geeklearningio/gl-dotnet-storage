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

        public string Name => this.innerStore.Name;

        public Task InitAsync() => this.innerStore.InitAsync();

        public Task DeleteAsync(IPrivateFileReference file) => this.innerStore.DeleteAsync(file);

        public Task<IFileReference> GetAsync(Uri file, bool withMetadata) => this.innerStore.GetAsync(file, withMetadata);

        public Task<IFileReference> GetAsync(IPrivateFileReference file, bool withMetadata) => this.innerStore.GetAsync(file, withMetadata);

        public Task<IFileReference[]> ListAsync(string path, bool recursive, bool withMetadata) => this.innerStore.ListAsync(path, recursive, withMetadata);

        public Task<IFileReference[]> ListAsync(string path, string searchPattern, bool recursive, bool withMetadata) => this.innerStore.ListAsync(path, searchPattern, recursive, withMetadata);

        public Task<byte[]> ReadAllBytesAsync(IPrivateFileReference file) => this.innerStore.ReadAllBytesAsync(file);

        public Task<string> ReadAllTextAsync(IPrivateFileReference file) => this.innerStore.ReadAllTextAsync(file);

        public Task<Stream> ReadAsync(IPrivateFileReference file) => this.innerStore.ReadAsync(file);

        public Task<IFileReference> SaveAsync(Stream data, IPrivateFileReference file, string contentType) => this.innerStore.SaveAsync(data, file, contentType);

        public Task<IFileReference> SaveAsync(byte[] data, IPrivateFileReference file, string contentType) => this.innerStore.SaveAsync(data, file, contentType);
    }
}
