using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryCategoryDtos;

public class ResultGalleryCategoryDto : IDto
{
    public Guid Id { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public string? DeletedBy { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>Many-to-many join rows linking this category to gallery items.</summary>
    public ICollection<GalleryItemCategory> GalleryCategories { get; set; } = [];
}
