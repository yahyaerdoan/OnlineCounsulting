using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.SocialMedia;

public record SocialMediaListItemViewModel(Guid Id, string Name, string Url, string Icon, string? IconColor, int DisplayOrder);

public class CreateSocialMediaViewModel
{
    [Required, MinLength(1)]
    public string Name { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public string Url { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public string Icon { get; set; } = string.Empty;

    public string? IconColor { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateSocialMediaViewModel : CreateSocialMediaViewModel
{
    public Guid Id { get; set; }
}
