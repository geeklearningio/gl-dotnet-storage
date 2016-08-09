namespace GeekLearning.Storage
{
    using GeekLearning.Storage;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;


    public static class IStoreExtensions
    {
        public static Task<IFileReference[]> ListAsync(this IStore store, string path, bool recursive = false, bool withMetadata = false)
        {
            return store.ListAsync(path, recursive: recursive, withMetadata: withMetadata);
        }

        public static Task<IFileReference[]> ListAsync(this IStore store, string path, string searchPattern, bool recursive = false, bool withMetadata = false)
        {
            return store.ListAsync(path, searchPattern, recursive: recursive, withMetadata: withMetadata);
        }

        public static Task DeleteAsync(this IStore store, string path)
        {
            return store.DeleteAsync(new Internal.PrivateFileReference(path));
        }

        public static Task<IFileReference> GetAsync(this IStore store, string path, bool withMetadata = false)
        {
            return store.GetAsync(new Internal.PrivateFileReference(path), withMetadata: withMetadata);
        }

        public static Task<Stream> ReadAsync(this IStore store, string path)
        {
            return store.ReadAsync(new Internal.PrivateFileReference(path));
        }

        public static Task<byte[]> ReadAllBytesAsync(this IStore store, string path)
        {
            return store.ReadAllBytesAsync(new Internal.PrivateFileReference(path));
        }

        public static Task<string> ReadAllTextAsync(this IStore store, string path)
        {
            return store.ReadAllTextAsync(new Internal.PrivateFileReference(path));
        }

        public static Task<IFileReference> SaveAsync(this IStore store, byte[] data, string path, string contentType)
        {
            return store.SaveAsync(data, new Internal.PrivateFileReference(path), contentType);
        }

        public static Task<IFileReference> SaveAsync(this IStore store, Stream data, string path, string contentType)
        {
            return store.SaveAsync(data, new Internal.PrivateFileReference(path), contentType);
        }
    }
}
