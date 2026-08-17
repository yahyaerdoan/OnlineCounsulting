using OnlineConsulting.UserInterface.Common;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Dashboard;

public class UserAccountViewModel
{
    public required UserSummaryViewModel User { get; set; }
    public ChangePasswordViewModel ChangePassword { get; set; } = new();
}

public class ChangePasswordViewModel
{
    public Guid Id { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
