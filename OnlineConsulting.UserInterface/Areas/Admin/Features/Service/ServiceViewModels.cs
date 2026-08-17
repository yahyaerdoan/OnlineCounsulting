using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Service;

public record CategoryOptionViewModel(Guid Id, string Title);

/// <summary>IsCover distinguishes the service's CoverMediaAssetId (Id is the media asset id) from a
/// ServiceMediaItem row (Id is the media item id) - the remove action handles both, see AdminServiceCatalogService.</summary>
public record ServiceImageViewModel(Guid Id, string Url, bool IsCover);

public record ServiceListItemViewModel(
    Guid Id,
    string Title,
    string CategoryTitle,
    string Description,
    string DetailedDescription,
    decimal Price,
    int DiscountRate,
    decimal DiscountedPrice,
    int TaxRate,
    bool FeaturedArea,
    string? CoverUrl);

public class CreateServiceViewModel
{
    [Required]
    public Guid CategoryId { get; set; }

    [Required, MinLength(1)]
    public string Title { get; set; } = string.Empty;

    [Required, MinLength(5)]
    public string Description { get; set; } = string.Empty;

    [Required, MinLength(5)]
    public string DetailedDescription { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, 100)]
    public int DiscountRate { get; set; }

    [Range(0, 100)]
    public int TaxRate { get; set; }

    public bool FeaturedArea { get; set; }

    public bool RequiresPrepayment { get; set; }

    /// <summary>Uploaded through IMediaService: the first one becomes the service's cover, the rest become
    /// gallery media items (what the old ServiceImage table used to hold).</summary>
    public List<IFormFile>? Images { get; set; }

    public List<CategoryOptionViewModel> Categories { get; set; } = [];
}

public class UpdateServiceViewModel : CreateServiceViewModel
{
    public Guid Id { get; set; }

    public List<ServiceImageViewModel> ExistingImages { get; set; } = [];
}
