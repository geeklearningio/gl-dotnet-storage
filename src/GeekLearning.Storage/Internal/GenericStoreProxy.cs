using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Storage.Internal
{
    public class GenericStoreProxy<TOptions> : IStore, IStore<TOptions>
        where TOptions : class, IStorageStoreOptions, new()
    {
        private IStore innerStore;

        public GenericStoreProxy(IStorageFactory factory, IOptions<TOptions> options)
        {
            this.innerStore = factory.GetStore(nameof(TOptions), options.Value);
        }

        public string Name
        {
            get
            {
                return innerStore.Name;
            }
        }

        public Task DeleteAsync(IPrivateFileReference file)
        {
            return innerStore.DeleteAsync(file);
        }

        public Task<IFileReference> GetAsync(Uri file, bool withMetadata)
        {
            return innerStore.GetAsync(file, withMetadata);
        }

        public Task<IFileReference> GetAsync(IPrivateFileReference file, bool withMetadata)
        {
            return innerStore.GetAsync(file, withMetadata);
        }

        public Task<IFileReference[]> ListAsync(string path, bool recursive, bool withMetadata)
        {
            return innerStore.ListAsync(path, recursive, withMetadata);
        }

        public Task<IFileReference[]> ListAsync(string path, string searchPattern, bool recursive, bool withMetadata)
        {
            return innerStore.ListAsync(path, searchPattern, recursive, withMetadata);
        }

        public Task<byte[]> ReadAllBytesAsync(IPrivateFileReference file)
        {
            return innerStore.ReadAllBytesAsync(file);
        }

        public Task<string> ReadAllTextAsync(IPrivateFileReference file)
        {
            return innerStore.ReadAllTextAsync(file);
        }

        public Task<Stream> ReadAsync(IPrivateFileReference file)
        {
            return innerStore.ReadAsync(file);
        }

        public Task<IFileReference> SaveAsync(Stream data, IPrivateFileReference file, string contentType)
        {
            return innerStore.SaveAsync(data, file, contentType);
        }

        public Task<IFileReference> SaveAsync(byte[] data, IPrivateFileReference file, string contentType)
        {
            return innerStore.SaveAsync(data, file, contentType);
        }
    }
}
