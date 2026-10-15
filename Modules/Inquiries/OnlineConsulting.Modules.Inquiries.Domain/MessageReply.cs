using Core.PersistenceLayer.MultiTenancy;

namespace OnlineConsulting.Modules.Inquiries.Domain;

/// <summary>One reply in a message's conversation, from staff or the customer.</summary>
public class MessageReply : SequentialGuidTenantEntity
{
    public required Guid MessageId { get; set; }
    public required string Body { get; set; }
    public bool IsFromCustomer { get; set; }
}
