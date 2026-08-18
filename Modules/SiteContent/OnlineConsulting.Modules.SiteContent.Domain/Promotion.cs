using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain;

public class Promotion : TenantEntity<Guid>
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string? CtaText { get; set; }
    public string? CtaUrl { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public int DisplayOrder { get; set; }
}
