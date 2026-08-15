namespace OnlineConsulting.Modules.Scheduling.Application.Features.Constants;

public static class SchedulingMessages
{
    public const string AppointmentNotFoundFormat = "Appointment {0} was not found.";
    public const string AvailabilityRuleNotFoundFormat = "Availability rule {0} was not found.";
    public const string SlotNoLongerAvailable = "The requested time slot is no longer available. Please pick another slot.";
    public const string InvalidTimeRange = "ScheduledEnd must be after ScheduledStart.";
    public const string OnlyPendingOrConfirmedCanBeCancelled = "Only pending or confirmed appointments can be cancelled.";
    public const string OnlyPendingCanBeConfirmed = "Only pending appointments can be confirmed.";
}
