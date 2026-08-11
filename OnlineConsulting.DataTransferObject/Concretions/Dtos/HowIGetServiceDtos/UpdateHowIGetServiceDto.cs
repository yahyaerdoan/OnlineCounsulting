using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.HowIGetServiceDtos;

public class UpdateHowIGetServiceDto : IDto
{
    public Guid Id { get; set; }
    public bool Status { get; set; }
    public Guid ImgIconId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
