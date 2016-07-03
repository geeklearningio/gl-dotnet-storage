namespace GeekLearning.Storage.Internal
{
    public class PrivateFileReference : IPrivateFileReference
    {
        public PrivateFileReference(string path)
        {
            this.Path = path;
        }
        public string Path { get; }
    }
}
