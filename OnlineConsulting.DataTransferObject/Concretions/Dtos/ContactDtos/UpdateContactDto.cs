using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.ContactDtos;

public class UpdateContactDto : IDto
{
    public Guid Id { get; set; }
    public bool Status { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string WorkingHours { get; set; } = string.Empty;
}
