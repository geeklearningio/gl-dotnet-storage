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

        public ValueTask<IFileReference> GetAsync(Uri file, bool withMetadata) => this.innerStore.GetAsync(file, withMetadata);

        public ValueTask<IFileReference> GetAsync(IPrivateFileReference file, bool withMetadata) => this.innerStore.GetAsync(file, withMetadata);

        public ValueTask<IFileReference[]> ListAsync(string path, bool recursive, bool withMetadata) => this.innerStore.ListAsync(path, recursive, withMetadata);

        public ValueTask<IFileReference[]> ListAsync(string path, string searchPattern, bool recursive, bool withMetadata) => this.innerStore.ListAsync(path, searchPattern, recursive, withMetadata);

        public ValueTask<byte[]> ReadAllBytesAsync(IPrivateFileReference file) => this.innerStore.ReadAllBytesAsync(file);

        public ValueTask<string> ReadAllTextAsync(IPrivateFileReference file) => this.innerStore.ReadAllTextAsync(file);

        public ValueTask<Stream> ReadAsync(IPrivateFileReference file) => this.innerStore.ReadAsync(file);

        public ValueTask<IFileReference> SaveAsync(Stream data, IPrivateFileReference file, string contentType) => this.innerStore.SaveAsync(data, file, contentType);

        public ValueTask<IFileReference> SaveAsync(byte[] data, IPrivateFileReference file, string contentType) => this.innerStore.SaveAsync(data, file, contentType);

        public ValueTask<string> GetSharedAccessSignatureAsync(ISharedAccessPolicy policy) => this.innerStore.GetSharedAccessSignatureAsync(policy);
    }
}
