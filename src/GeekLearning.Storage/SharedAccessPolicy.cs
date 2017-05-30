namespace GeekLearning.Storage
{
    using System;

    public class SharedAccessPolicy : ISharedAccessPolicy
    {
        public DateTimeOffset? StartTime { get; set; }

        public DateTimeOffset? ExpiryTime { get; set; }

        public SharedAccessPermissions Permissions { get; set; }
    }
}
