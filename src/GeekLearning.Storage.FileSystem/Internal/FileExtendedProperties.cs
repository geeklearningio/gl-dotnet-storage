namespace GeekLearning.Storage.FileSystem.Internal
{
    using System.Collections.Generic;

    public class FileExtendedProperties
    {
        public FileExtendedProperties()
        {
            this.Metadata = new Dictionary<string, string>();
        }

        public string ContentType { get; set; }

        public string ETag { get; set; }

        public IDictionary<string, string> Metadata { get; set; }
    }
}
