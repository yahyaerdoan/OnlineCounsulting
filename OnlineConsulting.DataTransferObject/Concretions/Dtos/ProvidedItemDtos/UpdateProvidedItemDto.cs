using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.ProvidedItemDtos;

public class UpdateProvidedItemDto : IDto
{
    public Guid Id { get; set; }
    public Guid ImgIconId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
