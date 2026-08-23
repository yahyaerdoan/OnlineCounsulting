namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

/// <summary>Shared login form model. RememberMe only affects the Web host's cookie persistence -
/// the MAUI head ignores it. No DataAnnotations - the API's FluentValidation rules are the source
/// of truth; the native "required" input attribute still covers a genuinely empty submit.</summary>
public class LoginModel
{
    public string UserNameOrEmail { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; } = true;
}
