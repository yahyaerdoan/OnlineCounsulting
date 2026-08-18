using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain;

/// <summary>Plain id, no navigation - Service lives in the Services module's own DbContext, same cross-module convention as everywhere else in this codebase.</summary>
public class FaqItem : TenantEntity<Guid>
{
    public required Guid ServiceId { get; set; }
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public int DisplayOrder { get; set; }
}
