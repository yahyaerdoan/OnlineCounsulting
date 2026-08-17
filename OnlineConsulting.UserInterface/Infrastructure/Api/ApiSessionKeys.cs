namespace OnlineConsulting.UserInterface.Infrastructure.Api;

/// <summary>ASP.NET Session keys used to carry the Api access token across requests, minted alongside the existing cookie sign-in on login (see AccountController). Session (not the auth cookie itself) so the token never round-trips to the browser.</summary>
public static class ApiSessionKeys
{
    public const string AccessToken = "Api:AccessToken";
}
