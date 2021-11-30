namespace GeekLearning.Storage.Azure.Internal
{
    using global::Azure.Storage.Blobs.Models;
    using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

    public class AzureListFileWrapper : FileInfoBase
    {
        private BlobItem blob;
        private string name;
        private AzureListDirectoryWrapper parent;

        public AzureListFileWrapper(BlobItem blob, AzureListDirectoryWrapper parent)
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

        public override string FullName => this.blob.Name;

        public override string Name => this.name;

        public override DirectoryInfoBase ParentDirectory => this.parent;
    }
}
