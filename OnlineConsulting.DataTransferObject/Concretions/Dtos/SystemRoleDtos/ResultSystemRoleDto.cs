namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.SystemRoleDtos;

public class ResultSystemRoleDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsExist { get; set; }
}
