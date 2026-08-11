using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.SocialMediaDtos;

public class UpdateSocialMediaDto : IDto
{
    public Guid Id { get; set; }
    public bool Status { get; set; }
    public Guid ClassIconId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
