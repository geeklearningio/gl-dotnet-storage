namespace GeekLearning.Storage.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class StoreOptions : IStoreOptions
    {
        private const string MissingPropertyErrorMessage = "{0} should be defined.";

        public string Name { get; set; }

        public string ProviderName { get; set; }

        public string ProviderType { get; set; }

        public AccessLevel AccessLevel { get; set; }

        public string FolderName { get; set; }
        
        public string AuthenticationMode { get; set; }

        public virtual IEnumerable<IOptionError> Validate(bool throwOnError = true)
        {
            var optionErrors = new List<OptionError>();

            if (string.IsNullOrEmpty(this.Name))
            {
                this.PushMissingPropertyError(optionErrors, nameof(this.Name));
            }

            if (string.IsNullOrEmpty(this.ProviderName) && string.IsNullOrEmpty(this.ProviderType))
            {
                optionErrors.Add(new OptionError
                {
                    PropertyName = "Provider",
                    ErrorMessage = $"You should set either a {nameof(this.ProviderType)} or a {nameof(this.ProviderName)} for each Store."
                });
            }

            if (string.IsNullOrEmpty(this.FolderName))
            {
                this.PushMissingPropertyError(optionErrors, nameof(this.FolderName));
            }

            if (throwOnError && optionErrors.Any())
            {
                throw new Exceptions.BadStoreConfiguration(this.Name, optionErrors);
            }

            return optionErrors;
        }

        protected void PushMissingPropertyError(List<OptionError> optionErrors, string propertyName)
        {
            optionErrors.Add(new OptionError
            {
                PropertyName = propertyName,
                ErrorMessage = string.Format(MissingPropertyErrorMessage, propertyName)
            });
        }
    }
}
