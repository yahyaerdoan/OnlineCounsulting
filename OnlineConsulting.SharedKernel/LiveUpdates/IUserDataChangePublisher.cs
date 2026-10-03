namespace OnlineConsulting.SharedKernel.LiveUpdates;

/// <summary>One user's data under one topic changed; clients refetch that topic from the Api - the signal carries no data.</summary>
public readonly record struct UserDataChange(Guid UserId, string Topic);

/// <summary>Delivers committed data changes to the affected users' connected clients (SignalR in the Api host).</summary>
public interface IUserDataChangePublisher
{
    Task PublishAsync(IReadOnlyCollection<UserDataChange> changes, CancellationToken cancellationToken = default);
}

/// <summary>Default when no real-time transport is registered (e.g. tools, tests) - changes are simply dropped.</summary>
public sealed class NullUserDataChangePublisher : IUserDataChangePublisher
{
    public Task PublishAsync(IReadOnlyCollection<UserDataChange> changes, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
