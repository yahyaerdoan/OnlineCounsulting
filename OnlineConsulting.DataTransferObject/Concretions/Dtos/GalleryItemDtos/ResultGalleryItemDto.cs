using Microsoft.AspNetCore.Http;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryItemDtos;

public class ResultGalleryItemDto : IDto
{
    public Guid Id { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? DeletedBy { get; set; }
    public bool Status { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public IFormFile Image { get; set; } = null!;
    public string? Description { get; set; }
}
