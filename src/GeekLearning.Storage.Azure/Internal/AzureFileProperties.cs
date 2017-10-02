namespace GeekLearning.Storage.Azure.Internal
{
    using Microsoft.WindowsAzure.Storage.Blob;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    public class AzureFileProperties : IFileProperties
    {
        private const string DefaultCacheControl = "max-age=300, must-revalidate";
        private readonly ICloudBlob cloudBlob;
        private readonly Dictionary<string, string> decodedMetadata;

        public AzureFileProperties(ICloudBlob cloudBlob)
        {
            this.cloudBlob = cloudBlob;
            if (string.IsNullOrEmpty(this.cloudBlob.Properties.CacheControl))
            {
                this.cloudBlob.Properties.CacheControl = DefaultCacheControl;
            }

            if (this.cloudBlob.Metadata != null)
            {
                decodedMetadata = this.cloudBlob.Metadata.ToDictionary(m => m.Key, m => WebUtility.HtmlDecode(m.Value));
            }
            else
            {
                decodedMetadata = new Dictionary<string, string>();
            }
        }

        public DateTimeOffset? LastModified => this.cloudBlob.Properties.LastModified;

        public long Length => this.cloudBlob.Properties.Length;

        public string ContentType
        {
            get { return this.cloudBlob.Properties.ContentType; }
            set { this.cloudBlob.Properties.ContentType = value; }
        }

        public string ETag => this.cloudBlob.Properties.ETag;

        public string CacheControl
        {
            get { return this.cloudBlob.Properties.CacheControl; }
            set { this.cloudBlob.Properties.CacheControl = value; }
        }

        public string ContentMD5 => this.cloudBlob.Properties.ContentMD5;

        public IDictionary<string, string> Metadata => this.decodedMetadata;

        internal async Task SaveAsync()
        {
            await this.cloudBlob.SetPropertiesAsync();

            foreach (var meta in this.decodedMetadata)
            {
                this.cloudBlob.Metadata[meta.Key] = WebUtility.HtmlEncode(meta.Value);
            }

            await this.cloudBlob.SetMetadataAsync();
        }
    }
}
