namespace GeekLearning.Storage.FileSystem.Configuration
{
    using GeekLearning.Storage.Configuration;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;

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

        public override IEnumerable<IOptionError> Validate(bool throwOnError = true)
        {
            var baseErrors = base.Validate(throwOnError);
            var optionErrors = new List<OptionError>();

            if (string.IsNullOrEmpty(this.AbsolutePath))
            {
                this.PushMissingPropertyError(optionErrors, nameof(this.AbsolutePath));
            }

            var finalErrors = baseErrors.Concat(optionErrors);
            if (throwOnError && finalErrors.Any())
            {
                throw new Exceptions.BadStoreConfiguration(this.Name, finalErrors);
            }

            return finalErrors;
        }
    }
}
