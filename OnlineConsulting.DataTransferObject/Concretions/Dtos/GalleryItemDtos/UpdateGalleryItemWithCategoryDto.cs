using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryCategoryDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryItemDtos;

public class UpdateGalleryItemWithCategoryDto : IDto
{
    public Guid Id { get; set; }
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
    public List<string> GalleryCategoryIds { get; set; } = [];
    public ICollection<ResultGalleryCategoryDto>? GalleryCategories { get; set; }
}
