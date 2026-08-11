using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;

public class UpdateUserDto : IDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public virtual DateTime? UpdatedDate { get; set; }
    public virtual string? UpdatedBy { get; set; }
    public bool Status { get; set; }
}
