namespace GeekLearning.Storage.FileSystem
{
    using System.Threading.Tasks;

    public interface IExtendedPropertiesProvider
    {
        Task<Internal.FileExtendedProperties> GetExtendedPropertiesAsync(string storeName, IPrivateFileReference file);

        Task SaveExtendedPropertiesAsync(string storeName, IPrivateFileReference file, Internal.FileExtendedProperties extendedProperties);
    }
}
