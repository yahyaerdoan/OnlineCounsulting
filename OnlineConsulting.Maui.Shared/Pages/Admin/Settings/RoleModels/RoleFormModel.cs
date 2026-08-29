namespace OnlineConsulting.Maui.Shared.Pages.Admin.Settings.RoleModels;

/// <summary>Shared by CreateRoleDialog and EditRoleDialog - both post the same Name/Description shape.</summary>
public class RoleFormModel
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
