namespace GeekLearning.Storage
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public interface IStore
    {
        string Name { get; }

        Task<IFileReference[]> ListAsync(string path, bool recursive, bool withMetadata);

        Task<IFileReference[]> ListAsync(string path, string searchPattern, bool recursive, bool withMetadata);

        Task<IFileReference> GetAsync(IPrivateFileReference file, bool withMetadata);

        Task<IFileReference> GetAsync(Uri file, bool withMetadata);

        Task DeleteAsync(IPrivateFileReference file);

        Task<Stream> ReadAsync(IPrivateFileReference file);

        Task<byte[]> ReadAllBytesAsync(IPrivateFileReference file);

        Task<string> ReadAllTextAsync(IPrivateFileReference file);

        Task<IFileReference> SaveAsync(byte[] data, IPrivateFileReference file, string contentType);

        Task<IFileReference> SaveAsync(Stream data, IPrivateFileReference file, string contentType);
    }
}
