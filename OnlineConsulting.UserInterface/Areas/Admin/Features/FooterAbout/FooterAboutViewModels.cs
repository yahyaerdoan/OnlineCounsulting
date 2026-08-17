using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.FooterAbout;

public record FooterAboutListItemViewModel(Guid Id, string Description, string ImageUrl);

public class CreateFooterAboutViewModel
{
    [Required, MinLength(5)]
    public string Description { get; set; } = string.Empty;

    public IFormFile? Image { get; set; }
}

public class UpdateFooterAboutViewModel : CreateFooterAboutViewModel
{
    public Guid Id { get; set; }
    public string? ImageUrl { get; set; }
}
