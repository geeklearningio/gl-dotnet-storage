namespace GeekLearning.Storage.Azure.Internal
{
    using Microsoft.WindowsAzure.Storage.Blob;
    using System;
    using System.Collections.Generic;

    public class AzureFileProperties : IFileProperties
    {
        private const string DefaultCacheControl = "max-age=300, must-revalidate";
        private readonly ICloudBlob cloudBlob;

        public AzureFileProperties(ICloudBlob cloudBlob)
        {
            this.cloudBlob = cloudBlob;
            if (string.IsNullOrEmpty(this.cloudBlob.Properties.CacheControl))
            {
                this.cloudBlob.Properties.CacheControl = DefaultCacheControl;
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

        public IDictionary<string, string> Metadata => this.cloudBlob.Metadata;
    }
}
