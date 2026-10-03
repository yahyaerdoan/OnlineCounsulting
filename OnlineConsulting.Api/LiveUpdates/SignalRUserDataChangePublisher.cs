using Microsoft.AspNetCore.SignalR;
using OnlineConsulting.SharedKernel.LiveUpdates;

namespace OnlineConsulting.Api.LiveUpdates;

/// <summary>Sends one "DataChanged" per (user, topic) to all of that user's connected clients.</summary>
public sealed class SignalRUserDataChangePublisher(IHubContext<UserUpdatesHub> hubContext) : IUserDataChangePublisher
{
    public Task PublishAsync(IReadOnlyCollection<UserDataChange> changes, CancellationToken cancellationToken = default) =>
        Task.WhenAll(changes.Distinct().Select(change =>
            hubContext.Clients.User(change.UserId.ToString()).SendAsync(UserUpdatesHub.DataChangedMethod, change.Topic, cancellationToken)));
}
