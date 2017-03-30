namespace GeekLearning.Storage.FileSystem.Server.Internal
{
    using FileSystem.Internal;
    using Microsoft.Extensions.Options;
    using System;

    public class PublicUrlProvider : IPublicUrlProvider
    {
        private FileSystemStorageServerOptions options;

        public PublicUrlProvider(IOptions<FileSystemStorageServerOptions> options)
        {
            this.options = options.Value;
        }

        public string GetPublicUrl(string storeName, FileSystemFileReference file)
        {
            var uriBuilder = new UriBuilder(options.BaseUri);
            uriBuilder.Path = options.EndpointPath.Add("/" + storeName).Add("/" + file.Path);

            return uriBuilder.ToString();
        }
    }
}
