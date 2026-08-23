namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Api route paths shared by every caller (Web host, MAUI head, both Login pages) so a
/// path never has to be retyped, and a route rename only touches one place.</summary>
public static class ApiRoutes
{
    public static class Auth
    {
        public const string Login = "/api/auth/login";
        public const string Refresh = "/api/auth/refresh";
    }

    public static class Users
    {
        public const string Me = "/api/users/me";
    }
}
