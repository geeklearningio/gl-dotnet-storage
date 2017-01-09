namespace GeekLearning.Storage.Azure.Internal
{
    using Microsoft.WindowsAzure.Storage.Blob;
    using System;
    using System.Collections.Generic;

    public class AzureFileProperties : IFileProperties
    {
        private ICloudBlob cloudBlob;

        public AzureFileProperties(ICloudBlob cloudBlob)
        {
            this.cloudBlob = cloudBlob;
        }

        public DateTimeOffset? LastModified => this.cloudBlob.Properties.LastModified;

        public string ContentType
        {
            get { return this.cloudBlob.Properties.ContentType; }
            set { this.cloudBlob.Properties.ContentType = value; }
        }

        public long Length => this.cloudBlob.Properties.Length;

        public string ETag => this.cloudBlob.Properties.ETag;

        public IDictionary<string, string> Metadata => this.cloudBlob.Metadata;
    }
}
