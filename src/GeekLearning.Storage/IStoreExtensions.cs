namespace GeekLearning.Storage
{
    using System.IO;
    using System.Threading.Tasks;

    public static class IStoreExtensions
    {
        public static Task<IFileReference[]> ListAsync(this IStore store, string path, bool recursive = false, bool withMetadata = false)
            => store.ListAsync(path, recursive: recursive, withMetadata: withMetadata);

        public static Task<IFileReference[]> ListAsync(this IStore store, string path, string searchPattern, bool recursive = false, bool withMetadata = false)
            => store.ListAsync(path, searchPattern, recursive: recursive, withMetadata: withMetadata);

        public static Task DeleteAsync(this IStore store, string path)
            => store.DeleteAsync(new Internal.PrivateFileReference(path));

        public static Task<IFileReference> GetAsync(this IStore store, string path, bool withMetadata = false)
            => store.GetAsync(new Internal.PrivateFileReference(path), withMetadata: withMetadata);

        public static Task<Stream> ReadAsync(this IStore store, string path)
            => store.ReadAsync(new Internal.PrivateFileReference(path));

        public static Task<byte[]> ReadAllBytesAsync(this IStore store, string path)
            => store.ReadAllBytesAsync(new Internal.PrivateFileReference(path));

        public static Task<string> ReadAllTextAsync(this IStore store, string path)
            => store.ReadAllTextAsync(new Internal.PrivateFileReference(path));

        public static Task<IFileReference> SaveAsync(this IStore store, byte[] data, string path, string contentType)
            => store.SaveAsync(data, new Internal.PrivateFileReference(path), contentType);

        public static Task<IFileReference> SaveAsync(this IStore store, Stream data, string path, string contentType)
            => store.SaveAsync(data, new Internal.PrivateFileReference(path), contentType);
    }
}
