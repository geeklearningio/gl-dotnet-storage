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
        private ICloudBlob blob;
        private string name;
        private AzureListDirectoryWrapper parent;

        public AzureListFileWrapper(ICloudBlob blob, AzureListDirectoryWrapper parent)
        {
            this.blob = blob;
            var lastSlash = blob.Name.LastIndexOf('/');
            if (lastSlash >= 0)
            {
                this.name = blob.Name.Substring(lastSlash + 1);
            }
            else
            {
                this.name = blob.Name;
            }
            this.parent = parent;
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
                return name;
            }
        }

        public override DirectoryInfoBase ParentDirectory
        {
            get
            {
                return this.parent;
            }
        }
    }
}
