namespace GeekLearning.Storage.FileSystem.Server
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System.Threading.Tasks;

    public class FileSystemStorageServerMiddleware
    {
        private RequestDelegate next;

        private ILogger<FileSystemStorageServerMiddleware> logger;
        private IOptions<FileSystemStorageServerOptions> serverOptions;
        private IOptions<StorageOptions> storageOptions;

        public FileSystemStorageServerMiddleware(RequestDelegate next,
            IOptions<FileSystemStorageServerOptions> serverOptions,
            ILogger<FileSystemStorageServerMiddleware> logger,
            IOptions<StorageOptions> storageOptions)
        {
            this.storageOptions = storageOptions;
            this.next = next;
            this.serverOptions = serverOptions;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var subPathStart = context.Request.Path.Value.IndexOf('/', 1);
            if (subPathStart > 0)
            {
                var storeName = context.Request.Path.Value.Substring(1, subPathStart - 1);
                var storageFactory = context.RequestServices.GetRequiredService<IStorageFactory>();

                StorageOptions.StorageStoreOptions storeOptions;
                if (this.storageOptions.Value.Stores.TryGetValue(storeName, out storeOptions)
                    && storeOptions.Provider == "FileSystem")
                {
                    string access;
                    // TODO: Fix options!
                    //if (!storeOptions.Parameters.TryGetValue("Access", out access) && access != "Public")
                    //{
                    //    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    //    return;
                    //}

                    IStore store = storageFactory.GetStore(storeName, storeOptions);

                    var file = await store.GetAsync(context.Request.Path.Value.Substring(subPathStart + 1), withMetadata: true);
                    if (file != null)
                    {
                        context.Response.ContentType = file.Properties.ContentType;
                        context.Response.StatusCode = StatusCodes.Status200OK;

                        if (!string.IsNullOrEmpty(file.Properties.ETag))
                        {
                            context.Response.Headers.Add("ETag", new[] { file.Properties.ETag });
                        }

                        await file.ReadToStreamAsync(context.Response.Body);
                        return;
                    }
                }
            }

            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }
    }
}
