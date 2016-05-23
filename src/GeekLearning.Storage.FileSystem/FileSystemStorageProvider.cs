using GeekLearning.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace GeekLearning.Storage.FileSystem
{
    public class FileSystemStorageProvider : IStorageProvider
    {
        private IHostingEnvironment appEnv;

        public FileSystemStorageProvider(IHostingEnvironment appEnv)
        {
            this.appEnv = appEnv;
        }

        public string Name
        {
            get
            {
                return "FileSystem";
            }
        }

        public IStore BuildStore(StorageOptions.StorageStore storeOptions)
        {
            return new FileSystemStore(storeOptions.Parameters["Path"], this.appEnv.ContentRootPath);
        }
    }
}
