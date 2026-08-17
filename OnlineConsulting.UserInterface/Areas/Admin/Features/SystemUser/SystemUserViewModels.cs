namespace OnlineConsulting.UserInterface.Areas.Admin.Features.SystemUser;

public record SystemUserRoleViewModel(string Name, string Description);

public record SystemUserListItemViewModel(Guid Id, string Username, string FirstName, string LastName, string Email, List<SystemUserRoleViewModel> Roles);

public class RoleAssignmentViewModel
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsAssigned { get; set; }
}
