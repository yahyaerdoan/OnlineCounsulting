using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryCategoryDtos;

public class UpdateGalleryCategoryDto : IDto
{
    public Guid Id { get; set; }
    public bool Status { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
