using Core.PersistenceLayer.MultiTenancy;

namespace OnlineConsulting.Modules.Inquiries.Domain;

public class Message : SequentialGuidTenantEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Subject { get; set; }
    public required string Description { get; set; }

    /// <summary>Signed-in sender; null for anonymous contact-form messages.</summary>
    public Guid? UserId { get; set; }

    /// <summary>Last staff reply; cleared when the customer writes again.</summary>
    public DateTimeOffset? RepliedAt { get; set; }

    /// <summary>Newest message in the conversation, for inbox ordering.</summary>
    public DateTimeOffset LastMessageAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Staff and customer replies, oldest first once ordered.</summary>
    public List<MessageReply> Replies { get; set; } = [];
}
