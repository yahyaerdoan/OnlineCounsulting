using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using System.Security.Claims;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Hubs;

/// <summary>Relays a technician's live GPS to the watching customer - positions are never persisted; group membership is re-validated on every join since AssignedTechnicianUserId can change.</summary>
[Authorize]
public class TechnicianTrackingHub(IAppointmentRepository appointmentRepository) : Hub<ITechnicianTrackingClient>
{
    /// <summary>Joins the caller to the appointment's tracking group. Caller must be the appointment's customer or its assigned technician.</summary>
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

    /// <summary>Removes the caller from the appointment's tracking group.</summary>
    public async Task LeaveAppointmentTracking(Guid appointmentId) =>
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(appointmentId));

    /// <summary>Broadcasts the caller's GPS position to the appointment's tracking group. Only the assigned technician may push, and is rate-limited by <see cref="TechnicianLocationThrottle"/>.</summary>
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

        await Clients.Group(GroupName(appointmentId)).ReceivedTechnicianLocation(new TechnicianLocationUpdate(appointmentId, latitude, longitude, DateTimeOffset.UtcNow));
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
