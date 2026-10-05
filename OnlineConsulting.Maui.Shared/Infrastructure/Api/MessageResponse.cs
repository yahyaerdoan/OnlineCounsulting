using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/inquiries/messages/query's response shape.</summary>
public record MessageResponse(Guid Id, string FirstName, string LastName, string Email, string Subject, string Description, DateTimeOffset CreatedDate, DateTimeOffset? RepliedAt) : HalResource, IQueryableFields
{
    public static string[] SearchFields => [nameof(FirstName), nameof(LastName), nameof(Email), nameof(Subject)];
}
