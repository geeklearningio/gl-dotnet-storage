namespace GeekLearning.Storage.FileSystem.Server
{
    using Microsoft.AspNetCore.Http;
    using System;

    public class FileSystemStorageServerOptions
    {
        public Uri BaseUri { get; set; }

        public PathString EndpointPath { get; set; } = "/.well-known/storage";

        public byte[] SigningKey { get; set; }
    }
}
