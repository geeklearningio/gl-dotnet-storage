namespace GeekLearning.Storage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public interface IFileReference : IPrivateFileReference
    {
        string PublicUrl { get; }

        DateTimeOffset? LastModified { get; }

        string ContentType { get; }

        long? Length { get; }

        string ETag { get; }

        IDictionary<string, string> Metadata { get; }

        Task ReadToStreamAsync(Stream targetStream);

        Task<Stream> ReadAsync();

        Task<string> ReadAllTextAsync();

        Task<byte[]> ReadAllBytesAsync();

        Task DeleteAsync();

        Task UpdateAsync(Stream stream);

        Task<string> GetExpirableUriAsync();

        Task AddMetadataAsync(IDictionary<string, string> metadata);

        Task SaveMetadataAsync();
    }
}
