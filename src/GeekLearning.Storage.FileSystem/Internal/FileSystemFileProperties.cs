namespace GeekLearning.Storage.FileSystem.Internal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FileSystemFileProperties : IFileProperties
    {
        private FileInfo fileInfo;

        public FileSystemFileProperties(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
        }

        public DateTimeOffset? LastModified => new DateTimeOffset(this.fileInfo.LastWriteTimeUtc, TimeZoneInfo.Local.BaseUtcOffset);

        public long Length => this.fileInfo.Length;

        public string ContentType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string ETag
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IDictionary<string, string> Metadata
        {
            get
            {
                throw new NotImplementedException();
            }
        }


    }
}
