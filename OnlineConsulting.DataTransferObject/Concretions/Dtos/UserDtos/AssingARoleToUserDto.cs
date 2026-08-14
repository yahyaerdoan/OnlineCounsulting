namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;

public class AssingARoleToUserDto
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsAssigned { get; set; }
}
