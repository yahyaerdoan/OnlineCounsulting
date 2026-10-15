using OnlineConsulting.Modules.Inquiries.Domain;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;

public class MessageResponse
{
    public required Guid Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string Subject { get; init; }
    public required string Description { get; init; }
    public required DateTimeOffset CreatedDate { get; init; }

    /// <summary>Signed-in sender; null for anonymous contact-form messages.</summary>
    public Guid? UserId { get; init; }
    public DateTimeOffset? RepliedAt { get; init; }
    public required DateTimeOffset LastMessageAt { get; init; }

    /// <summary>Filled by GetMessage only; empty in lists.</summary>
    public IReadOnlyList<MessageReplyResponse> Replies { get; init; } = [];

    public static MessageResponse FromDomain(Message message) => new()
    {
        Id = message.Id,
        FirstName = message.FirstName,
        LastName = message.LastName,
        Email = message.Email,
        Subject = message.Subject,
        Description = message.Description,
        CreatedDate = message.CreatedDate,
        UserId = message.UserId,
        RepliedAt = message.RepliedAt,
        LastMessageAt = message.LastMessageAt,
        Replies = [.. message.Replies.Select(MessageReplyResponse.FromDomain)],
    };
}
