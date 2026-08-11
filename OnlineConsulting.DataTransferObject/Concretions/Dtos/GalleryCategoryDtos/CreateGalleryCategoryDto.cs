using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryCategoryDtos;

public class CreateGalleryCategoryDto : IDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation property for many-to-many relationship
}
