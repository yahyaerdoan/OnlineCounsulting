using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.SystemRole;

public record SystemRoleListItemViewModel(Guid Id, string Name, string Description);

public class CreateSystemRoleViewModel
{
    [Required, MinLength(1)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}

public class UpdateSystemRoleViewModel : CreateSystemRoleViewModel
{
    public Guid Id { get; set; }
}

public record PermissionCheckboxViewModel(string Value, bool IsChecked);

public record AssignRolePermissionsViewModel(Guid RoleId, string RoleName, Dictionary<string, List<PermissionCheckboxViewModel>> PermissionsByModule);
