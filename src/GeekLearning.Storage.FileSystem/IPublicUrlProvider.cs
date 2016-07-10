using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Storage.FileSystem
{
    public interface IPublicUrlProvider
    {
        string GetPublicUrl(Internal.FileSystemFileReference file);
    }
}
