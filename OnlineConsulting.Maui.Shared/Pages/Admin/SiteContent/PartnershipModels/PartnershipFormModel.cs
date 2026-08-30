namespace OnlineConsulting.Maui.Shared.Pages.Admin.SiteContent.PartnershipModels;

/// <summary>Shared by CreatePartnershipDialog and EditPartnershipDialog. Social links are managed separately, only in EditPartnershipDialog.</summary>
public class PartnershipFormModel
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string WebsiteUrl { get; set; } = string.Empty;

    public Guid? PhotoMediaAssetId { get; set; }

    public int DisplayOrder { get; set; }
}
