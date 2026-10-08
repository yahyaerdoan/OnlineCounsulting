using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Scheduling.Domain;

/// <summary>A customer's requested service visit. State changes only through its methods, which throw when called in the wrong state.</summary>
public class Appointment : SequentialGuidTenantEntity
{
    private Appointment()
    {
    }

    /// <summary>Identity module user id, no navigation.</summary>
    public Guid UserId { get; private set; }

    /// <summary>Catalog service id, no navigation; null for a general meeting request.</summary>
    public Guid? ServiceId { get; private set; }

    public DateTimeOffset ScheduledStart { get; private set; }
    public DateTimeOffset ScheduledEnd { get; private set; }

    /// <summary>One of <see cref="AppointmentStatuses"/>.</summary>
    public string Status { get; private set; } = AppointmentStatuses.Pending;

    public string? CustomerNote { get; private set; }

    /// <summary>One of <see cref="AppointmentMeetingTypes"/>.</summary>
    public string MeetingType { get; private set; } = AppointmentMeetingTypes.InPerson;

    /// <summary>What an online meeting is about; null for in-person visits.</summary>
    public string? Topic { get; private set; }

    /// <summary>Free-text address of an in-person visit; null for online meetings.</summary>
    public string? ServiceAddress { get; private set; }

    /// <summary>Reserved for prepaid bookings; always false for now.</summary>
    public bool RequiresPrepayment { get; private set; }

    /// <summary>Identity module user id of the dispatched technician; allows them to share live location for this visit.</summary>
    public Guid? AssignedTechnicianUserId { get; private set; }

    /// <summary>Cancelled or completed.</summary>
    public bool IsClosed => AppointmentRules.IsClosed(Status);

    /// <summary>Pending, so staff can confirm it.</summary>
    public bool CanBeConfirmed => AppointmentRules.CanBeConfirmed(Status);

    /// <summary>Pending or confirmed, so it can be cancelled.</summary>
    public bool CanBeCancelled => AppointmentRules.CanBeCancelled(Status);

    /// <summary>Creates a pending visit; keeps the address for in-person visits and the topic for online meetings.</summary>
    public static Appointment Request(Guid userId, Guid? serviceId, DateTimeOffset scheduledStart, DateTimeOffset scheduledEnd, string meetingType, string? topic, string? serviceAddress, string? customerNote)
    {
        if (scheduledEnd <= scheduledStart)
        {
            throw new ArgumentException("A visit must end after it starts.", nameof(scheduledEnd));
        }

        if (!AppointmentMeetingTypes.All.Contains(meetingType))
        {
            throw new ArgumentException($"Unknown meeting type '{meetingType}'.", nameof(meetingType));
        }

        var online = meetingType == AppointmentMeetingTypes.Online;

        if (online)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(topic);
        }
        else
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(serviceAddress);
        }

        return new Appointment
        {
            UserId = userId,
            ServiceId = serviceId,
            ScheduledStart = scheduledStart,
            ScheduledEnd = scheduledEnd,
            MeetingType = meetingType,
            Topic = online ? topic?.Trim() : null,
            ServiceAddress = online ? null : serviceAddress?.Trim(),
            CustomerNote = customerNote,
        };
    }

    /// <summary>Requires <see cref="CanBeConfirmed"/>.</summary>
    public void Confirm()
    {
        if (!CanBeConfirmed)
        {
            throw new InvalidOperationException($"Only a pending appointment can be confirmed, but {Id} is {Status}.");
        }

        Status = AppointmentStatuses.Confirmed;
    }

    /// <summary>Requires <see cref="CanBeCancelled"/>.</summary>
    public void Cancel()
    {
        if (!CanBeCancelled)
        {
            throw new InvalidOperationException($"Only a pending or confirmed appointment can be cancelled, but {Id} is {Status}.");
        }

        Status = AppointmentStatuses.Cancelled;
    }

    /// <summary>Marks the visit done once its work order is recorded. Requires the visit not to be <see cref="IsClosed"/>.</summary>
    public void Complete()
    {
        EnsureActive(nameof(Complete));
        Status = AppointmentStatuses.Completed;
    }

    /// <summary>Dispatches a technician, replacing any earlier one. Requires the visit not to be <see cref="IsClosed"/>.</summary>
    public void AssignTechnician(Guid technicianUserId)
    {
        EnsureActive(nameof(AssignTechnician));
        AssignedTechnicianUserId = technicianUserId;
    }

    private void EnsureActive(string action)
    {
        if (IsClosed)
        {
            throw new InvalidOperationException($"{action} needs an active appointment, but {Id} is {Status}.");
        }
    }
}
