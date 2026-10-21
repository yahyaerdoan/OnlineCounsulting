namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

public static class AuthClaimTypes
{
    /// <summary>Claim type the Web host uses to smuggle the Api access token inside the sign-in
    /// cookie, since Blazor Server circuits don't reliably keep ASP.NET Core Session state alive.</summary>
    public const string AccessToken = "onlineconsulting:access_token";

    public const string RefreshToken = "onlineconsulting:refresh_token";

    public const string AccessTokenExpiresAt = "onlineconsulting:access_token_expires_at";

    public const string IsSuperAdmin = "onlineconsulting:is_super_admin";

    /// <summary>A module the user holds any permission in (the part before the dot, e.g. "scheduling").</summary>
    public const string PermissionArea = "onlineconsulting:permission_area";
}
