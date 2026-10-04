namespace OnlineConsulting.Modules.Scheduling.Domain;

/// <summary>Values of <see cref="Appointment.MeetingType"/>: InPerson needs a service address, Online needs a topic.</summary>
public static class AppointmentMeetingTypes
{
    public const string InPerson = "InPerson";
    public const string Online = "Online";

    public static readonly string[] All = [InPerson, Online];
}
