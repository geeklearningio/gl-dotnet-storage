﻿using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace GeekLearning.Storage.Azure.Internal
{
    public class AzureListDirectoryWrapper : DirectoryInfoBase
    {
        private string name;
        private string fullName;
        private string path;
        private Dictionary<string, AzureFileReference> files;

        public AzureListDirectoryWrapper(FileSystemInfoBase childrens)
        {
            this.fullName = "root";
            this.ParentDirectory = null;
        }

        public AzureListDirectoryWrapper(string path, Dictionary<string, AzureFileReference> files)
        {
            this.path = path;
            this.files = files;
            this.fullName = path;
            var lastSlash = path.LastIndexOf('/');
            if (lastSlash >= 0)
            {
                this.name = path.Substring(lastSlash + 1);
            }
            else
            {
                this.name = path;
            }
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
                return name;
            }
        }

        public override DirectoryInfoBase ParentDirectory
        {
            get;
        }

        public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos()
        {
            return this.files.Values.Select(file => new AzureListFileWrapper(file.CloudBlob, this));
        }

        public override DirectoryInfoBase GetDirectory(string path)
        {
            throw new NotImplementedException();
        }

        public override FileInfoBase GetFile(string path)
        {
            return new AzureListFileWrapper(this.files[path].CloudBlob, this);
        }
    }
}
