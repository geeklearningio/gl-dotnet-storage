namespace GeekLearning.Storage
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IStore
    {
        Task<string[]> List(string path);

        Task Delete(string path);

        Task<Stream> Read(string path);

        Task<byte[]> ReadAllBytes(string path);

        Task<string> ReadAllText(string path);

        Task<string> Save(byte[] data, string path, string mimeType);

        Task<string> Save(Stream data, string path, string mimeType);

        Task<string> GetExpirableUri(string uri);
    }
}
