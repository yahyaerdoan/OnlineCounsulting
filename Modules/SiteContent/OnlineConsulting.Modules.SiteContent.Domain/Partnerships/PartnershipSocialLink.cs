using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain.Partnerships;

public class PartnershipSocialLink : TenantEntity<Guid>
{
    public required Guid PartnershipId { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }

    /// <summary>MudBlazor icon value (e.g. Icons.Material.Filled.LinkedIn) - a plain portable string, not a reference to the legacy ClassIcon lookup table (which is shared with the separate SocialMedia entity and stays untouched). Same pattern as Category.Icon.</summary>
    public required string Icon { get; set; }

    /// <summary>Hex color applied to Icon. Null means the frontend uses its default theme color.</summary>
    public string? IconColor { get; set; }
}
