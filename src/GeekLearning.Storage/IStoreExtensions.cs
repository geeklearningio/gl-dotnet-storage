namespace GeekLearning.Storage
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public static class IStoreExtensions
    {
        public static ValueTask<IFileReference[]> ListAsync(this IStore store, string path, bool recursive = false, bool withMetadata = false)
            => store.ListAsync(path, recursive: recursive, withMetadata: withMetadata);

        public static ValueTask<IFileReference[]> ListAsync(this IStore store, string path, string searchPattern, bool recursive = false, bool withMetadata = false)
            => store.ListAsync(path, searchPattern, recursive: recursive, withMetadata: withMetadata);

        public static Task DeleteAsync(this IStore store, string path)
            => store.DeleteAsync(new Internal.PrivateFileReference(path));

        public static ValueTask<IFileReference> GetAsync(this IStore store, string path, bool withMetadata = false)
            => store.GetAsync(new Internal.PrivateFileReference(path), withMetadata: withMetadata);

        public static ValueTask<Stream> ReadAsync(this IStore store, string path)
            => store.ReadAsync(new Internal.PrivateFileReference(path));

        public static ValueTask<byte[]> ReadAllBytesAsync(this IStore store, string path)
            => store.ReadAllBytesAsync(new Internal.PrivateFileReference(path));

        public static ValueTask<string> ReadAllTextAsync(this IStore store, string path)
            => store.ReadAllTextAsync(new Internal.PrivateFileReference(path));

        public static ValueTask<IFileReference> SaveAsync(this IStore store, byte[] data, string path, string contentType, OverwritePolicy overwritePolicy = OverwritePolicy.Always, IDictionary<string,string> metadata = null)
            => store.SaveAsync(data, new Internal.PrivateFileReference(path), contentType, overwritePolicy, metadata);

        public static ValueTask<IFileReference> SaveAsync(this IStore store, Stream data, string path, string contentType, OverwritePolicy overwritePolicy = OverwritePolicy.Always, IDictionary<string,string> metadata = null)
            => store.SaveAsync(data, new Internal.PrivateFileReference(path), contentType, overwritePolicy, metadata);
    }
}
