using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.SystemRoleDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;

public class ResultUserDto : IDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public List<ResultSystemRoleDto> Roles { get; set; } = [];
}
