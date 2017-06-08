namespace GeekLearning.Storage.Exceptions
{
    using System;

    public class FileAlreadyExistsException : Exception
    {
        public FileAlreadyExistsException(string storeName, string filePath)
            : base($"The file {filePath} already exists in Store {storeName}.")
        {
        }
    }
}
