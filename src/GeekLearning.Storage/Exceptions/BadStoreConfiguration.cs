namespace GeekLearning.Storage.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BadStoreConfiguration : Exception
    {
        public BadStoreConfiguration(string storeName) 
            : base($"The store '{storeName}' was not properly configured.")
        {
        }

        public BadStoreConfiguration(string storeName, string details)
            : base($"The store '{storeName}' was not properly configured. {details}")
        {
        }

        public BadStoreConfiguration(string storeName, IEnumerable<Configuration.IOptionError> errors)
            : this(storeName, string.Join(" | ", errors.Select(e => e.ErrorMessage)))
        {
            this.Errors = errors;
        }

        public IEnumerable<Configuration.IOptionError> Errors { get; }
    }
}
