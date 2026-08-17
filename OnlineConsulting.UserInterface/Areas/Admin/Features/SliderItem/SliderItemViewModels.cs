using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.SliderItem;

public record SliderItemListItemViewModel(Guid Id, string Title, string Description, string ImageUrl);

public class CreateSliderItemViewModel
{
    [Required, MinLength(1)]
    public string Title { get; set; } = string.Empty;

    [Required, MinLength(5)]
    public string Description { get; set; } = string.Empty;

    public IFormFile? Image { get; set; }
}

public class UpdateSliderItemViewModel : CreateSliderItemViewModel
{
    public Guid Id { get; set; }
    public string? ImageUrl { get; set; }
}
