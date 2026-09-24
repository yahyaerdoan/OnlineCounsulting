using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain.Partnerships;

public class PartnershipSocialLink : SequentialGuidTenantEntity
{
    public required Guid PartnershipId { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }

    /// <summary>MudBlazor icon value, not the legacy ClassIcon lookup table (shared with SocialMedia, left untouched) - same pattern as Category.Icon.</summary>
    public required string Icon { get; set; }

    /// <summary>Hex color applied to Icon. Null means the frontend uses its default theme color.</summary>
    public string? IconColor { get; set; }
}
