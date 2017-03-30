namespace GeekLearning.Storage.FileSystem
{
    public interface IPublicUrlProvider
    {
        string GetPublicUrl(string storeName, Internal.FileSystemFileReference file);
    }
}
