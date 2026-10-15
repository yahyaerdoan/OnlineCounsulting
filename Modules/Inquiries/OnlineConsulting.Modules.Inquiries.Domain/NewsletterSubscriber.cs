using Core.PersistenceLayer.MultiTenancy;

namespace OnlineConsulting.Modules.Inquiries.Domain;

public class NewsletterSubscriber : SequentialGuidTenantEntity
{
    public required string Email { get; set; }
}
