namespace GeekLearning.Storage
{
    using System;
    using System.Collections.Generic;

    public interface IFileProperties
    {
        DateTimeOffset? LastModified { get; }

        long Length { get; }

        string ContentType { get; set; }

        string ETag { get; }

        IDictionary<string, string> Metadata { get; }
    }
}
