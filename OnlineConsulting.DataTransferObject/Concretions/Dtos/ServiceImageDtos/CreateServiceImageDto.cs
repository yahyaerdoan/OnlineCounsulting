using Microsoft.AspNetCore.Http;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceImageDtos;

public class CreateServiceImageDto : IDto
{
    public Guid? ServiceId { get; set; }
    public IFormFile Image { get; set; } = null!;
    public string? ImageUrl { get; set; }
}
