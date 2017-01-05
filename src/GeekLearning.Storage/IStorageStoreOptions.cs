namespace GeekLearning.Storage
{
    using System.Collections.Generic;

    public interface IStorageStoreOptions
    {
        string Provider { get; }

        Dictionary<string, string> Parameters { get; }
    }
}
