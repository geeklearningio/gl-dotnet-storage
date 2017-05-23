namespace GeekLearning.Storage
{
    using System;

    public interface ISharedAccessPolicy
    {
        DateTimeOffset? StartTime { get; }

        DateTimeOffset? ExpiryTime { get; }

        SharedAccessPermissions Permissions { get; }
    }
}
