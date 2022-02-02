namespace GeekLearning.Storage.Azure.Configuration
{
    using GeekLearning.Storage.Configuration;
    using System.Collections.Generic;
    using System.Linq;

    public class AzureStoreOptions : StoreOptions
    {
        public string ConnectionString { get; set; }

        public string AuthenticationMode { get; set; }

        public string ConnectionStringName { get; set; }

        public override IEnumerable<IOptionError> Validate(bool throwOnError = true)
        {
            var baseErrors = base.Validate(throwOnError);
            var optionErrors = new List<OptionError>();

            if (string.IsNullOrEmpty(this.ConnectionString))
            {
                this.PushMissingPropertyError(optionErrors, nameof(this.ConnectionString));
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
