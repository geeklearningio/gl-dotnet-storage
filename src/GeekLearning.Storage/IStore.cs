namespace GeekLearning.Storage
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public interface IStore
    {
        Task<IFileReference[]> ListAsync(string path, bool recursive);

        Task<IFileReference[]> ListAsync(string path, string searchPattern, bool recursive);

        Task<IFileReference> GetAsync(IPrivateFileReference file);

        Task<IFileReference> GetAsync(Uri file);

        Task DeleteAsync(IPrivateFileReference file);

        Task<Stream> ReadAsync(IPrivateFileReference file);

        Task<byte[]> ReadAllBytesAsync(IPrivateFileReference file);

        Task<string> ReadAllTextAsync(IPrivateFileReference file);

        Task<IFileReference> SaveAsync(byte[] data, IPrivateFileReference file, string mimeType);

        Task<IFileReference> SaveAsync(Stream data, IPrivateFileReference file, string mimeType);
    }
}
