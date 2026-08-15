using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Inquiries.Domain;

public class NewsletterSubscriber : TenantEntity<Guid>
{
    public required string Email { get; set; }
}
