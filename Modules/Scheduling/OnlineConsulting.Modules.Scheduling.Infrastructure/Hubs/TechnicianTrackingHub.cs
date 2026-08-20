using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using System.Security.Claims;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Hubs;

/// <summary>Relays a technician's live GPS position to the customer watching a specific appointment. Purely a real-time relay - positions are never persisted (a WorkOrder/Appointment row isn't the right place for a stream of GPS pings, and nothing downstream needs history of them). Group membership is re-validated against the current Appointment on every join, not cached, since AssignedTechnicianUserId can change between connections.</summary>
[Authorize]
public class TechnicianTrackingHub(IAppointmentRepository appointmentRepository) : Hub
{
    public async Task JoinAppointmentTracking(Guid appointmentId)
    {
        var userId = GetUserId();
        var appointment = await appointmentRepository.GetAsync(a => a.Id == appointmentId);
        if (appointment is null || (appointment.UserId != userId && appointment.AssignedTechnicianUserId != userId))
        {
            throw new HubException("Not authorized for this appointment.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(appointmentId));
    }

    public async Task LeaveAppointmentTracking(Guid appointmentId) =>
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(appointmentId));

    public async Task PushLocation(Guid appointmentId, double latitude, double longitude)
    {
        if (!TechnicianLocationThrottle.TryAcquire(Context.ConnectionId))
        {
            throw new HubException("Too many location updates - please slow down.");
        }

        var userId = GetUserId();
        var appointment = await appointmentRepository.GetAsync(a => a.Id == appointmentId);
        if (appointment is null || appointment.AssignedTechnicianUserId != userId)
        {
            throw new HubException("Not authorized to push a location for this appointment.");
        }

        await Clients.Group(GroupName(appointmentId)).SendAsync("ReceivedTechnicianLocation", new
        {
            appointmentId,
            latitude,
            longitude,
            timestamp = DateTimeOffset.UtcNow,
        });
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        TechnicianLocationThrottle.Release(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    internal static string GroupName(Guid appointmentId) => $"appointment-tracking-{appointmentId}";

    private Guid GetUserId()
    {
        var value = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(value, out var userId) ? userId : throw new HubException("Missing user id claim.");
    }
}
