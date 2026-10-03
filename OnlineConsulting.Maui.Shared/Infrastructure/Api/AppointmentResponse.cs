namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors Scheduling's AppointmentResponse - MediaItems deliberately omitted, the admin dispatch list doesn't need the customer's media gallery.</summary>
public record AppointmentResponse(Guid Id, Guid UserId, Guid? ServiceId, DateTimeOffset ScheduledStart, DateTimeOffset ScheduledEnd,
    string Status, string? CustomerNote, bool RequiresPrepayment, Guid? AssignedTechnicianUserId, string? ServiceAddress, string? NavigationUrl,
    string MeetingType = "InPerson", string? Topic = null) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Status)];

    public bool IsOnline => MeetingType == "Online";
}
