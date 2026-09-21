namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

/// <summary>RememberMe only affects Web's cookie persistence (MAUI ignores it); no DataAnnotations - the API's FluentValidation is the source of truth.</summary>
public class LoginModel
{
    public string UserNameOrEmail { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; } = true;
}
