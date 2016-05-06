using GeekLearning.Storage;
using Microsoft.Extensions.OptionsModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;

namespace GeekLearning.Storage.FileSystem
{
    public class FileSystemStorageProvider : IStorageProvider
    {
        private IApplicationEnvironment appEnv;

        public FileSystemStorageProvider(IApplicationEnvironment appEnv)
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
            return new FileSystemStore(storeOptions.Parameters["Path"], this.appEnv.ApplicationBasePath);
        }
    }
}
