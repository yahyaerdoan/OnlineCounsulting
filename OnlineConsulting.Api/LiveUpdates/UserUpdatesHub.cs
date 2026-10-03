using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace OnlineConsulting.Api.LiveUpdates;

/// <summary>Server-to-client only: a signed-in client connects and receives "DataChanged(topic)" whenever its user's data is
/// committed - by itself, another device, an admin, a payment webhook or a background job. Delivery is by user id
/// (Clients.User), so every device of that user gets it; no client-callable methods.</summary>
[Authorize]
public sealed class UserUpdatesHub : Hub
{
    public const string Path = "/hubs/user-updates";

    public const string DataChangedMethod = "DataChanged";
}
