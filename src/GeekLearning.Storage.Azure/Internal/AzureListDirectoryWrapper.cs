using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace GeekLearning.Storage.Azure.Internal
{
    public class AzureListDirectoryWrapper : DirectoryInfoBase
    {
        private string fullName;

        public AzureListDirectoryWrapper(FileSystemInfoBase childrens)
        {
            this.fullName = "root";
            this.ParentDirectory = null;
        }

        public AzureListDirectoryWrapper(CloudBlobDirectory blobDirectory, AzureListDirectoryWrapper parent = null)
        {
            this.ParentDirectory = parent;
            this.fullName = blobDirectory.Prefix;
        }

        public override string FullName
        {
            get
            {
                return fullName;
            }
        }

        public override string Name
        {
            get
            {
                return fullName;
            }
        }

        public override DirectoryInfoBase ParentDirectory
        {
            get;
        }

        public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos()
        {
            throw new NotImplementedException();
        }

        public override DirectoryInfoBase GetDirectory(string path)
        {
            throw new NotImplementedException();
        }

        public override FileInfoBase GetFile(string path)
        {
            throw new NotImplementedException();
        }
    }
}
