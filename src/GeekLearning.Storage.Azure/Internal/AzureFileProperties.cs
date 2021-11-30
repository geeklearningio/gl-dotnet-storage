namespace GeekLearning.Storage.Azure.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using global::Azure.Storage.Blobs;
    using global::Azure.Storage.Blobs.Models;

    public class AzureFileProperties : IFileProperties
    {
        private const string DefaultCacheControl = "max-age=300, must-revalidate";
        private readonly BlobClient client;
        private readonly Dictionary<string, string> decodedMetadata;


        public AzureFileProperties(BlobClient client, BlobProperties blobProperties)
        {
            this.client = client;
            this.LastModified = blobProperties.LastModified;
            this.Length = blobProperties.ContentLength;
            this.ContentType = blobProperties.ContentType;
            this.ContentDisposition = blobProperties.ContentDisposition;
            this.ContentEncoding = blobProperties.ContentEncoding;
            this.CacheControl = blobProperties.CacheControl;
            this.ContentMD5 = Convert.ToBase64String(blobProperties.ContentHash);
            this.ETag = blobProperties.ETag.ToString();

            if (blobProperties.Metadata != null)
            {
                decodedMetadata = blobProperties.Metadata.ToDictionary(m => m.Key, m => WebUtility.HtmlDecode(m.Value));
            }
            else
            {
                decodedMetadata = new Dictionary<string, string>();
            }
        }

        public AzureFileProperties(BlobClient client, BlobItem cloudBlob)
        {
            this.client = client;
            this.LastModified = cloudBlob.Properties.LastModified;
            this.Length = cloudBlob.Properties.ContentLength.GetValueOrDefault(0);
            this.ContentType = cloudBlob.Properties.ContentType;
            this.ContentDisposition = cloudBlob.Properties.ContentDisposition;
            this.ContentEncoding = cloudBlob.Properties.ContentEncoding;
            this.CacheControl = cloudBlob.Properties.CacheControl;
            this.ContentMD5 = Convert.ToBase64String(cloudBlob.Properties.ContentHash);
            this.ETag = cloudBlob.Properties.ETag.ToString();

            if (cloudBlob.Metadata != null)
            {
                decodedMetadata = cloudBlob.Metadata.ToDictionary(m => m.Key, m => WebUtility.HtmlDecode(m.Value));
            }
            else
            {
                decodedMetadata = new Dictionary<string, string>();
            }
        }

        public DateTimeOffset? LastModified { get; }

        public long Length { get; }

        public string ContentType { get; set; }

        public string ETag { get; set; }

        public string CacheControl { get; set; }

        public string ContentDisposition { get; set; }

        public string ContentEncoding { get; set; }

        public string ContentMD5 { get; }

        public IDictionary<string, string> Metadata => this.decodedMetadata;

        internal async Task SaveAsync()
        {
            await client.SetHttpHeadersAsync(new BlobHttpHeaders
            {
                ContentType = ContentType,
                CacheControl = CacheControl,
                ContentDisposition = ContentDisposition,
                ContentEncoding = ContentEncoding,
                ContentHash = Convert.FromBase64String(ContentMD5),
            });

            foreach (var meta in this.decodedMetadata)
            {
                this.Metadata[meta.Key] = WebUtility.HtmlEncode(meta.Value);
            }

            await client.SetMetadataAsync(this.Metadata);
        }
    }
}
