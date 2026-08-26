namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Api route paths shared by every caller (Web host, MAUI head, both Login pages) so a
/// path never has to be retyped, and a route rename only touches one place.</summary>
public static class ApiRoutes
{
    /// <summary>?index=&amp;size= for any paginated /query endpoint.</summary>
    public static string Paged(string basePath, int index, int size) => $"{basePath}?index={index}&size={size}";

    public static class Auth
    {
        public const string Login = "/api/auth/login";
        public const string Refresh = "/api/auth/refresh";
    }

    public static class Users
    {
        public const string Me = "/api/users/me";
        public const string All = "/api/users/query";

        public static string ById(Guid id) => $"/api/users/{id}";
        public static string Roles(Guid id) => $"/api/users/{id}/roles";
    }
}
