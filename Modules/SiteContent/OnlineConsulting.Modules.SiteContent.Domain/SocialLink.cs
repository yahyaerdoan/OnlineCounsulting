using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain;

public class SocialLink : TenantEntity<Guid>
{
    public required string Name { get; set; }
    public required string Url { get; set; }

    /// <summary>Portable icon string (e.g. a CSS font-icon class), not a reference to the legacy ClassIcon lookup table. Same pattern as PartnershipSocialLink.Icon.</summary>
    public required string Icon { get; set; }

    /// <summary>Hex color applied to Icon. Null means the frontend uses its default theme color.</summary>
    public string? IconColor { get; set; }
    public int DisplayOrder { get; set; }
}
