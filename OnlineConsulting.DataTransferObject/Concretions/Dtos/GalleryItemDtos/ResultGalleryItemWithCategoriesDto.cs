using Microsoft.AspNetCore.Http;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryItemDtos;

public class ResultGalleryItemWithCategoriesDto : IDto
{
    public Guid Id { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public string? DeletedBy { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public IFormFile Image { get; set; } = null!;
    public string? Description { get; set; }
    public ICollection<GalleryCategory> GalleryCategories { get; set; } = [];
}
