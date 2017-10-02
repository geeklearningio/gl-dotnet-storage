namespace GeekLearning.Storage.Configuration
{
    using System.Collections.Generic;

    public interface IStoreOptions : INamedElementOptions
    {
        string ProviderName { get; set; }

        string ProviderType { get; set; }

        AccessLevel AccessLevel { get; set; }

        string FolderName { get; set; }

        IEnumerable<IOptionError> Validate(bool throwOnError = true);
    }
}
