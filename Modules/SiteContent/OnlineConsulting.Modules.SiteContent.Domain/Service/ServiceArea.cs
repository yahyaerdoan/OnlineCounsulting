using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain.Service;

public class ServiceArea : SequentialGuidTenantEntity
{
    public required string Name { get; set; }
    public required string State { get; set; }
    public required string Slug { get; set; }
    public string? IntroText { get; set; }
    public int DisplayOrder { get; set; }
}
