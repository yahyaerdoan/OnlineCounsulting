using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.PartnershipSocialMediaDtos;

public class UpdatePartnershipSocialMediaDto : IDto
{
    public Guid Id { get; set; }
    public Guid ClassIconId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public Guid PartnershipId { get; set; }
}
