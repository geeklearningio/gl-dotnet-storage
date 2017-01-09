namespace GeekLearning.Storage.FileSystem.Internal
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class FileSystemFileProperties : IFileProperties
    {
        private readonly FileInfo fileInfo;
        private readonly FileExtendedProperties extendedProperties;

        public FileSystemFileProperties(string fileSystemPath, FileExtendedProperties extendedProperties)
        {
            this.fileInfo = new FileInfo(fileSystemPath);
            this.extendedProperties = extendedProperties;
        }

        public DateTimeOffset? LastModified => new DateTimeOffset(this.fileInfo.LastWriteTimeUtc, TimeZoneInfo.Local.BaseUtcOffset);

        public long Length => this.fileInfo.Length;

        public string ContentType
        {
            get { return this.extendedProperties.ContentType; }
            set { this.extendedProperties.ContentType = value; }
        }

        public string ETag => this.extendedProperties.ETag;

        public IDictionary<string, string> Metadata => this.extendedProperties.Metadata;

        internal FileExtendedProperties ExtendedProperties => this.extendedProperties;
    }
}
