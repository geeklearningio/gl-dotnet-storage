using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Storage.BasicSample
{
    public class TemplatesStore : StoreBase
    {
        public TemplatesStore(IStorageFactory storageFactory) : base("Templates", storageFactory)
        {
        }
    }
}
