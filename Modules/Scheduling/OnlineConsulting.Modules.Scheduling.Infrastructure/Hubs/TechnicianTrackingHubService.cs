using Microsoft.AspNetCore.SignalR;
using OnlineConsulting.Modules.Scheduling.Application.Features.TechnicianTracking.Abstractions;
using OnlineConsulting.SharedKernel.Notifications;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Hubs;

/// <summary>Notifies both channels a customer might be reached through: SignalR (only reaches an actively-open app) and push (reaches the device even when the app is closed) - complementary, not redundant.</summary>
public class TechnicianTrackingHubService(IHubContext<TechnicianTrackingHub, ITechnicianTrackingClient> hubContext, IPushNotificationSender pushNotificationSender) : ITechnicianTrackingHubService
{
    public async Task NotifyTechnicianAssignedAsync(Guid appointmentId, Guid customerUserId, Guid technicianUserId, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.Group(TechnicianTrackingHub.GroupName(appointmentId))
            .ReceivedTechnicianAssigned(new TechnicianAssignedUpdate(appointmentId, technicianUserId));

        await pushNotificationSender.SendToUserAsync(customerUserId, "Your technician is on the way", "A technician has been assigned to your appointment.", new Dictionary<string, string> { ["appointmentId"] = appointmentId.ToString() }, cancellationToken);
    }
}
