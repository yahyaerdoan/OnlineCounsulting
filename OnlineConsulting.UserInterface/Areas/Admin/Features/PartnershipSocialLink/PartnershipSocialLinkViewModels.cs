using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.PartnershipSocialLink;

public record PartnershipSocialLinkListItemViewModel(Guid Id, Guid PartnershipId, string Name, string Url, string Icon, string? IconColor);

public class CreatePartnershipSocialLinkViewModel
{
    public Guid PartnershipId { get; set; }

    [Required, MinLength(1)]
    public string Name { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public string Url { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public string Icon { get; set; } = string.Empty;

    public string? IconColor { get; set; }
}

public class UpdatePartnershipSocialLinkViewModel : CreatePartnershipSocialLinkViewModel
{
    public Guid Id { get; set; }
}
