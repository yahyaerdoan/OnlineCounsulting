using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.GalleryCategory;

public record GalleryCategoryListItemViewModel(Guid Id, string Name, string? Description);

public class CreateGalleryCategoryViewModel
{
    [Required, MinLength(1)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}

public class UpdateGalleryCategoryViewModel : CreateGalleryCategoryViewModel
{
    public Guid Id { get; set; }
}
