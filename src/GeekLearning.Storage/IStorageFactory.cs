using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Storage
{
    public interface IStorageFactory
    {
        IStore GetStore(string store);
    }
}
