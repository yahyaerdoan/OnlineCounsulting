namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/inquiries/messages/query's response shape.</summary>
public record MessageResponse(Guid Id, string FirstName, string LastName, string Email, string Subject, string Description, DateTimeOffset CreatedDate, DateTimeOffset? RepliedAt) : IQueryableFields
{
    public static string[] SearchFields => [nameof(FirstName), nameof(LastName), nameof(Email), nameof(Subject)];
}
