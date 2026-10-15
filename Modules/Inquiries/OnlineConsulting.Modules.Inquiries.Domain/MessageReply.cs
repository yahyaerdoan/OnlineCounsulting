using Core.PersistenceLayer.MultiTenancy;

namespace OnlineConsulting.Modules.Inquiries.Domain;

/// <summary>One staff reply to a message, emailed to the sender.</summary>
public class MessageReply : SequentialGuidTenantEntity
{
    public required Guid MessageId { get; set; }
    public required string Body { get; set; }
}
