using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors the inquiries message endpoints' response shape (staff inbox and the customer's own).</summary>
public record MessageResponse(Guid Id, string FirstName, string LastName, string Email, string Subject, string Description, DateTimeOffset CreatedDate, DateTimeOffset? RepliedAt, DateTimeOffset LastMessageAt) : HalResource, IQueryableFields
{
    public static string[] SearchFields => [nameof(FirstName), nameof(LastName), nameof(Email), nameof(Subject), nameof(Description)];

    /// <summary>Filled by GET ById only; empty in lists.</summary>
    public IReadOnlyList<MessageReplyResponse> Replies { get; init; } = [];

    /// <summary>Signed-in sender; null for anonymous contact-form messages.</summary>
    public Guid? UserId { get; init; }

    public string FullName => $"{FirstName} {LastName}";
}

public record MessageReplyResponse(Guid Id, string Body, bool IsFromCustomer, DateTimeOffset CreatedDate);
