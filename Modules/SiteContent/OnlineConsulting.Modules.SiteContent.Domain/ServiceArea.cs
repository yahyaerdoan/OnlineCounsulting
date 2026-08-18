using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain;

public class ServiceArea : TenantEntity<Guid>
{
    public required string Name { get; set; }
    public required string State { get; set; }
    public required string Slug { get; set; }
    public string? IntroText { get; set; }
    public int DisplayOrder { get; set; }
}
