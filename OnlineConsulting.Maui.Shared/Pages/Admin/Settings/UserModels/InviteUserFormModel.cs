namespace OnlineConsulting.Maui.Shared.Pages.Admin.Settings.UserModels;

public class InviteUserFormModel
{
    public string Email { get; set; } = string.Empty;

    /// <summary>Matches GlobalOperationClaims.Member on the API.</summary>
    public string RoleName { get; set; } = "Member";
}
