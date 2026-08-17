using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.AboutUs;

public record AboutUsListItemViewModel(Guid Id, string Title, string Description, string? CoverImage, string? VideoUrl);

public class CreateAboutUsViewModel
{
    [Required, MinLength(1)]
    public string Title { get; set; } = string.Empty;

    [Required, MinLength(5)]
    public string Description { get; set; } = string.Empty;

    public string? VideoUrl { get; set; }

    public IFormFile? Image { get; set; }
}

public class UpdateAboutUsViewModel : CreateAboutUsViewModel
{
    public Guid Id { get; set; }
    public string? CoverImage { get; set; }
}
