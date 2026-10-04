namespace OnlineConsulting.Modules.Scheduling.Domain;

/// <summary>Values of <see cref="Appointment.Status"/>.</summary>
public static class AppointmentStatuses
{
    public const string Pending = "Pending";
    public const string Confirmed = "Confirmed";
    public const string Cancelled = "Cancelled";
    public const string Completed = "Completed";

    /// <summary>Reserved for prepaid bookings; not produced yet.</summary>
    public const string PendingPayment = "PendingPayment";
}
