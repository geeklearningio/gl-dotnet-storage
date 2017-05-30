namespace GeekLearning.Storage
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public interface IStore
    {
        string Name { get; }

        Task InitAsync();

        ValueTask<IFileReference[]> ListAsync(string path, bool recursive, bool withMetadata);

        ValueTask<IFileReference[]> ListAsync(string path, string searchPattern, bool recursive, bool withMetadata);

        ValueTask<IFileReference> GetAsync(IPrivateFileReference file, bool withMetadata);

        ValueTask<IFileReference> GetAsync(Uri file, bool withMetadata);

        Task DeleteAsync(IPrivateFileReference file);

        ValueTask<Stream> ReadAsync(IPrivateFileReference file);

        ValueTask<byte[]> ReadAllBytesAsync(IPrivateFileReference file);

        ValueTask<string> ReadAllTextAsync(IPrivateFileReference file);

        ValueTask<IFileReference> SaveAsync(byte[] data, IPrivateFileReference file, string contentType);

        ValueTask<IFileReference> SaveAsync(Stream data, IPrivateFileReference file, string contentType);

        ValueTask<string> GetSharedAccessSignatureAsync(ISharedAccessPolicy policy);
    }
}
