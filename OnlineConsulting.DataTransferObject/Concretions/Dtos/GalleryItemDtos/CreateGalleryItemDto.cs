using Microsoft.AspNetCore.Http;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryItemDtos;

public class CreateGalleryItemDto : IDto
{
    public string? ImageUrl { get; set; }
    public IFormFile? Image { get; set; }
    public string? Description { get; set; }
    public List<string> GalleryCategoryIds { get; set; } = [];
    public ICollection<GalleryCategory>? GalleryCategories { get; set; }
}
