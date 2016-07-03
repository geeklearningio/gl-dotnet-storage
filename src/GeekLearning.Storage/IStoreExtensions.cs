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
        public static Task DeleteAsync(this IStore store, string path)
        {
            return store.DeleteAsync(new Internal.PrivateFileReference(path));
        }

        public static Task<IFileReference> GetAsync(this IStore store, string path)
        {
            return store.GetAsync(new Internal.PrivateFileReference(path));
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

        public static Task<IFileReference> SaveAsync(this IStore store, byte[] data, string path, string mimeType)
        {
            return store.SaveAsync(data, new Internal.PrivateFileReference(path), mimeType);
        }

        public static Task<IFileReference> SaveAsync(this IStore store, Stream data, string path, string mimeType)
        {
            return store.SaveAsync(data, new Internal.PrivateFileReference(path), mimeType);
        }
    }
}
