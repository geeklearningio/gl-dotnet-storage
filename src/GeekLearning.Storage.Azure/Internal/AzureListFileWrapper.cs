namespace GeekLearning.Storage.Azure.Internal
{
    using global::Azure.Storage.Blobs.Models;
    using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

    public class AzureListFileWrapper : FileInfoBase
    {
        private string name;
        private readonly string blobName;
        private AzureListDirectoryWrapper parent;

        public AzureListFileWrapper(string blobName, AzureListDirectoryWrapper parent)
        {
            var lastSlash = blobName.LastIndexOf('/');
            if (lastSlash >= 0)
            {
                this.name = blobName.Substring(lastSlash + 1);
            }
            else
            {
                this.name = blobName;
            }

            this.blobName = blobName;
            this.parent = parent;
        }

        public override string FullName => this.blobName;

        public override string Name => this.name;

        public override DirectoryInfoBase ParentDirectory => this.parent;
    }
}
