namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Constants;

/// <summary>Where the visit happens - the customer picks it on every booking. InPerson needs a service address; Online needs a topic.</summary>
public static class AppointmentMeetingTypes
{
    public const string InPerson = "InPerson";
    public const string Online = "Online";

    public static readonly string[] All = [InPerson, Online];
}
