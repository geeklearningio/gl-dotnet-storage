using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Storage
{
    public interface IStorageProvider
    {
        string Name { get; }


        IStore BuildStore(StorageOptions.StorageStore storeOptions);
    }
}
