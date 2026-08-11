using Microsoft.AspNetCore.Http;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.AboutUsDtos;

public class CreateAboutUsDto : IDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
    public string? CoverImage { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
}
