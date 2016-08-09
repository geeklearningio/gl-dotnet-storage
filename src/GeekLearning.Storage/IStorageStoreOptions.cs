using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Storage
{
    public interface IStorageStoreOptions
    {
        string Provider { get; }
        Dictionary<string, string> Parameters { get; }
    }
}
