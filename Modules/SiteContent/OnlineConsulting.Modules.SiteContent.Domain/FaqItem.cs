using Core.PersistenceLayer.MultiTenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain;

/// <summary>Plain id, no navigation - Service lives in the Services module's own DbContext, same cross-module convention as everywhere else in this codebase.
/// No ServiceId means a general, business-wide question.</summary>
public class FaqItem : SequentialGuidTenantEntity
{
    public Guid? ServiceId { get; set; }
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public int DisplayOrder { get; set; }
}
