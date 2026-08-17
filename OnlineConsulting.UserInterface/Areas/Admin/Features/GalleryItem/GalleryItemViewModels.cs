using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.GalleryItem;

public record GalleryCategoryOptionViewModel(Guid Id, string Name);

public record GalleryItemListItemViewModel(Guid Id, string Description, string? ImageUrl, List<string> CategoryNames);

public class CreateGalleryItemViewModel
{
    [Required, MinLength(1)]
    public string Description { get; set; } = string.Empty;

    /// <summary>The Api requires at least one tag (CreateGalleryItemValidator), preserving the legacy rule.</summary>
    public List<Guid> GalleryCategoryIds { get; set; } = [];

    public IFormFile? Image { get; set; }

    public int DisplayOrder { get; set; }

    public List<GalleryCategoryOptionViewModel> AvailableCategories { get; set; } = [];
}

public class UpdateGalleryItemViewModel : CreateGalleryItemViewModel
{
    public Guid Id { get; set; }

    /// <summary>Already-stored photo: kept when the edit form doesn't upload a replacement.</summary>
    public Guid? PhotoMediaAssetId { get; set; }

    public string? ImageUrl { get; set; }
}
