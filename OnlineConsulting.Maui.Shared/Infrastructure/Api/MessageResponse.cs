using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/inquiries/messages/query's response shape.</summary>
public record MessageResponse(Guid Id, string FirstName, string LastName, string Email, string Subject, string Description, DateTimeOffset CreatedDate, DateTimeOffset? RepliedAt) : HalResource, IQueryableFields
{
    public static string[] SearchFields => [nameof(FirstName), nameof(LastName), nameof(Email), nameof(Subject), nameof(Description)];

    /// <summary>Filled by GET ById only; empty in lists.</summary>
    public IReadOnlyList<MessageReplyResponse> Replies { get; init; } = [];

    public string FullName => $"{FirstName} {LastName}";
}

public record MessageReplyResponse(Guid Id, string Body, DateTimeOffset CreatedDate);
