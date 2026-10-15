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
    public DateTimeOffset? RepliedAt { get; init; }

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
        RepliedAt = message.RepliedAt,
        Replies = [.. message.Replies.Select(MessageReplyResponse.FromDomain)],
    };
}

public record MessageReplyResponse(Guid Id, string Body, DateTimeOffset CreatedDate)
{
    public static MessageReplyResponse FromDomain(MessageReply reply) => new(reply.Id, reply.Body, reply.CreatedDate);
}
