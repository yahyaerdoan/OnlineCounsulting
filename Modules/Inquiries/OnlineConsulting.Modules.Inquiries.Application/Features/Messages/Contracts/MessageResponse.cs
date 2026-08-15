using OnlineConsulting.Modules.Inquiries.Domain;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;

public record MessageResponse(Guid Id, string FirstName, string LastName, string Email, string Subject, string Description, DateTimeOffset CreatedDate)
{
    public static MessageResponse FromDomain(Message message) => new(
        message.Id, message.FirstName, message.LastName, message.Email, message.Subject, message.Description, message.CreatedDate);
}
