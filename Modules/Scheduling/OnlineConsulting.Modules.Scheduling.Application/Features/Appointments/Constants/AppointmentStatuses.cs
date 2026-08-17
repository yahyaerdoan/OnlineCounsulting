namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Constants;

public static class AppointmentStatuses
{
    public const string Pending = "Pending";
    public const string Confirmed = "Confirmed";
    public const string Cancelled = "Cancelled";
    public const string Completed = "Completed";

    /// <summary>Not produced yet - reserved for when a real payment gateway lands and RequiresPrepayment starts gating confirmation on a paid Commerce order.</summary>
    public const string PendingPayment = "PendingPayment";
}
