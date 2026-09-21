namespace OnlineConsulting.UserInterface.Infrastructure.Api;

/// <summary>Session key (not the auth cookie itself) so the Api access token never round-trips to the browser.</summary>
public static class ApiSessionKeys
{
    public const string AccessToken = "Api:AccessToken";
}
