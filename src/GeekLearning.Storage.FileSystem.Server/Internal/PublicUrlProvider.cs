namespace GeekLearning.Storage.FileSystem.Server.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GeekLearning.Storage.FileSystem.Internal;
    using Microsoft.Extensions.Options;

    public class PublicUrlProvider : IPublicUrlProvider
    {
        private FileSystemStorageServerOptions options;

        public PublicUrlProvider(IOptions<FileSystemStorageServerOptions> options)
        {
            this.options = options.Value;
        }

        public string GetPublicUrl(FileSystemFileReference file)
        {
            var uriBuilder = new UriBuilder(options.BaseUri);
            uriBuilder.Path = options.EndpointPath.Add("/" + file.Path);

            return uriBuilder.ToString();
        }
    }
}
