using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Partnership;

/// <summary>PhotoUrl is resolved separately from the Media module (GetMediaAsset) since PartnershipResponse only carries the MediaAssetId, not a display-ready Url - the Api's own cross-module convention (plain id, no navigation) applies here too.</summary>
public record PartnershipListItemViewModel(Guid Id, string FirstName, string LastName, string Title, string Description, string Email, string WebsiteUrl, string? PhotoUrl);

public record PartnershipSocialLinkViewModel(Guid Id, string Name, string Url, string Icon, string? IconColor);

/// <summary>Used by the public Home page showcase partial only - includes CompanyName (not on the leaner
/// PartnershipListItemViewModel) and each partner's social links.</summary>
public record PartnershipShowcaseItemViewModel(Guid Id, string FirstName, string LastName, string Title, string Description, string Email, string CompanyName, string WebsiteUrl, string? PhotoUrl, List<PartnershipSocialLinkViewModel> SocialLinks);

public class CreatePartnershipViewModel
{
    [Required, MinLength(1)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string CompanyName { get; set; } = string.Empty;

    [Required, Url]
    public string WebsiteUrl { get; set; } = string.Empty;

    [Required, MinLength(5)]
    public string Title { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(5)]
    public string Description { get; set; } = string.Empty;

    /// <summary>Optional on create too, matching the Api's own CreatePartnershipCommand (PhotoMediaAssetId is nullable) - a partnership can be added before its photo is ready.</summary>
    public IFormFile? Image { get; set; }
}

public class UpdatePartnershipViewModel : CreatePartnershipViewModel
{
    public Guid Id { get; set; }
    public string? PhotoUrl { get; set; }
}
