namespace GeekLearning.Storage.FileSystem.Configuration
{
    using GeekLearning.Storage.Configuration;
    using System.IO;

    public class FileSystemStoreOptions : StoreOptions
    {
        public string RootPath { get; set; }

        public string AbsolutePath
        {
            get
            {
                if (string.IsNullOrEmpty(this.RootPath))
                {
                    return this.FolderName;
                }

                if (string.IsNullOrEmpty(this.FolderName))
                {
                    return this.RootPath;
                }

                return Path.Combine(this.RootPath, this.FolderName);
            }
        }
    }
}
