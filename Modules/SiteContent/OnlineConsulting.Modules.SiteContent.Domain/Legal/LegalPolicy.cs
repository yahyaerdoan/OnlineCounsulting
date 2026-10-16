using Core.PersistenceLayer.MultiTenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain.Legal;

/// <summary>A site's privacy policy and terms of service, one row per site; plain text, blank lines split paragraphs.</summary>
public class LegalPolicy : SequentialGuidTenantEntity
{
    public string? PrivacyPolicy { get; set; }
    public string? TermsOfService { get; set; }
}
