using Hateoas;
using OnlineConsulting.Modules.Inquiries.Domain;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;

/// <summary>Message response as a class with required init properties, since records can't inherit the plain LinkedResponse class.</summary>
public class MessageResponse : LinkedResponse
{
    public required Guid Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string Subject { get; init; }
    public required string Description { get; init; }
    public required DateTimeOffset CreatedDate { get; init; }
    public DateTimeOffset? RepliedAt { get; init; }

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
    };
}
