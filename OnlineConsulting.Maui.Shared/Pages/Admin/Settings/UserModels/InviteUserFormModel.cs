namespace OnlineConsulting.Maui.Shared.Pages.Admin.Settings.UserModels;

public class InviteUserFormModel
{
    public string Email { get; set; } = string.Empty;

    public string RoleName { get; set; } = "Member"; // matches GlobalOperationClaims.Member on the API
}
