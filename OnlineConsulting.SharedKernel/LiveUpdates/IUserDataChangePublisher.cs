namespace OnlineConsulting.SharedKernel.LiveUpdates;

/// <summary>Delivers committed data changes to the affected users' connected clients (SignalR in the Api host).</summary>
public interface IUserDataChangePublisher
{
    /// <summary>Signals each affected user's connected clients; users who aren't connected aren't reached.</summary>
    Task PublishAsync(IReadOnlyCollection<UserDataChange> changes, CancellationToken cancellationToken = default);
}
