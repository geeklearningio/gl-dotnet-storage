namespace GeekLearning.Storage
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IFileReference: IPrivateFileReference
    {
        string PublicUrl { get; }

        Task<Stream> ReadAsync();

        Task DeleteAsync();

        Task UpdateAsync(Stream stream);

        Task<string> GetExpirableUriAsync();
    }
}
