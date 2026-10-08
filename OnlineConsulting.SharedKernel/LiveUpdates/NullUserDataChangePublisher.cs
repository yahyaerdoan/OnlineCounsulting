namespace OnlineConsulting.SharedKernel.LiveUpdates;

/// <summary>Default when no real-time transport is registered (e.g. tools, tests) - changes are simply dropped.</summary>
public sealed class NullUserDataChangePublisher : IUserDataChangePublisher
{
    public Task PublishAsync(IReadOnlyCollection<UserDataChange> changes, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
