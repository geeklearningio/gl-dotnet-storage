namespace GeekLearning.Storage.Azure.Internal
{
    using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
            this.path = path ?? "";
            this.files = files;
            this.fullName = this.path;
            var lastSlash = this.path.LastIndexOf('/');
            if (lastSlash >= 0)
            {
                this.name = path.Substring(lastSlash + 1);
            }
            else
            {
                this.name = path;
            }
        }

        public override string FullName => this.fullName;

        public override string Name => this.name;

        public override DirectoryInfoBase ParentDirectory { get; }

        public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos()
        {
            return this.files.Values.Select(file => new AzureListFileWrapper(file.Path, this));
        }

        public override DirectoryInfoBase GetDirectory(string path)
        {
            throw new NotImplementedException();
        }

        public override FileInfoBase GetFile(string path)
        {
            return new AzureListFileWrapper(this.files[path].Path, this);
        }
    }
}
