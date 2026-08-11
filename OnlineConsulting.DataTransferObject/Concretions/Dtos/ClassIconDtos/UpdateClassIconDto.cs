using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.ClassIconDtos;

public class UpdateClassIconDto : IDto
{
    public Guid Id { get; set; }
    public bool Status { get; set; }
    public string IconClass { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
