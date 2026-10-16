using OnlineConsulting.Modules.Inquiries.Domain;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;

public record MessageReplyResponse(Guid Id, string Body, bool IsFromCustomer, DateTimeOffset CreatedDate)
{
    public static MessageReplyResponse FromDomain(MessageReply reply) => new(reply.Id, reply.Body, reply.IsFromCustomer, reply.CreatedDate);
}
