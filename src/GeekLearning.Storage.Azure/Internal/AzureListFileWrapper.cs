using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Storage.Azure.Internal
{
    public class AzureListFileWrapper : FileInfoBase
    {
        private CloudBlob blob;

        public AzureListFileWrapper(CloudBlob blob)
        {
            this.blob = blob;
        }

        public override string FullName
        {
            get
            {
                return this.blob.Name;
            }
        }

        public override string Name
        {
            get
            {
                return this.blob.Name;
            }
        }

        public override DirectoryInfoBase ParentDirectory
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
