namespace OnlineConsulting.Modules.Scheduling.Domain;

/// <summary>Appointment lifecycle rules shared by <see cref="Appointment"/> and code that only has the status (e.g. HATEOAS links).</summary>
public static class AppointmentRules
{
    /// <summary>Cancelled or completed: nothing more can happen to it.</summary>
    public static bool IsClosed(string status) => status is AppointmentStatuses.Cancelled or AppointmentStatuses.Completed;

    /// <summary>Only a pending visit can be confirmed.</summary>
    public static bool CanBeConfirmed(string status) => status == AppointmentStatuses.Pending;

    /// <summary>Pending or confirmed visits can be cancelled by the customer or staff.</summary>
    public static bool CanBeCancelled(string status) => status is AppointmentStatuses.Pending or AppointmentStatuses.Confirmed;
}
