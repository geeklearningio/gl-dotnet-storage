namespace GeekLearning.Storage.FileSystem
{
    using System.Threading.Tasks;

    public interface IExtendedPropertiesProvider
    {
        Task<Internal.FileExtendedProperties> GetExtendedPropertiesAsync(string storeAbsolutePath, IPrivateFileReference file);

        Task SaveExtendedPropertiesAsync(string storeAbsolutePath, IPrivateFileReference file, Internal.FileExtendedProperties extendedProperties);
    }
}
