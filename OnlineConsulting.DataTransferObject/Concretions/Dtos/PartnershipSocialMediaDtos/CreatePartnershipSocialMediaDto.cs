using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.PartnershipSocialMediaDtos;

public class CreatePartnershipSocialMediaDto : IDto
{
    public Guid ClassIconId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string PartnershipId { get; set; } = string.Empty;
    public ClassIcon ClassIcon { get; set; } = null!;
}
