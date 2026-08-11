using Microsoft.AspNetCore.Http;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceImageDtos;

public class UpdateServiceImageDto : IDto
{
    public Guid Id { get; set; }
    public Guid ServiceId { get; set; }
    public IFormFile? Image { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
