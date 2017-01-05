namespace GeekLearning.Storage.Internal
{
    public class PrivateFileReference : IPrivateFileReference
    {
        public PrivateFileReference(string path)
        {
            this.Path = path.Replace("\\", "/").TrimStart('/');
        }

        public string Path { get; }
    }
}
