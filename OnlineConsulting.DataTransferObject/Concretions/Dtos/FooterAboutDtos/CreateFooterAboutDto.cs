using Microsoft.AspNetCore.Http;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.FooterAboutDtos;

public class CreateFooterAboutDto : IDto
{
    public string? ImageUrl { get; set; }
    public string Description { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
}
