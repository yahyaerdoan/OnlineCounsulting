using Microsoft.AspNetCore.Http;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.ImgIconDtos;

public class UpdateImgIconDto : IDto
{
    public Guid Id { get; set; }
    public bool Status { get; set; }
    public string IconUrl { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IFormFile? IconImage { get; set; }
}
