namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Api route paths, shared by every caller so a rename touches one place.</summary>
public static class ApiRoutes
{
    /// <summary>?index=&amp;size= for any paginated /query endpoint.</summary>
    public static string Paged(string basePath, int index, int size) => $"{basePath}?index={index}&size={size}";

    public static class Auth
    {
        public const string Login = "/api/auth/login";
        public const string Refresh = "/api/auth/refresh";
        public const string AcceptInvite = "/api/auth/invites/accept";
    }

    /// <summary>Invite-a-teammate route - the invitee sets their own password, not the admin.</summary>
    public static class Invites
    {
        public const string Create = "/api/auth/invites";
        public const string All = "/api/invites/query";

        public static string ById(Guid id) => $"/api/invites/{id}";
    }

    public static class Permissions
    {
        public const string All = "/api/permissions";
    }

    public static class Users
    {
        public const string Me = "/api/users/me";
        public const string All = "/api/users/query";

        public static string ById(Guid id) => $"/api/users/{id}";
        public static string Roles(Guid id) => $"/api/users/{id}/roles";
    }

    public static class Roles
    {
        public const string All = "/api/roles/query";

        /// <summary>GET for the flat dropdown list, POST for create.</summary>
        public const string Base = "/api/roles";

        public static string ById(Guid id) => $"/api/roles/{id}";
        public static string Permissions(Guid id) => $"/api/roles/{id}/permissions";

        /// <summary>Every role's permissions in one call - backs the permission matrix page.</summary>
        public const string PermissionsMatrix = "/api/roles/permissions";
    }

    public static class Categories
    {
        public const string All = "/api/categories/query";
        public const string Base = "/api/categories";

        public static string ById(Guid id) => $"/api/categories/{id}";
    }

    public static class Services
    {
        public const string All = "/api/services/query";
        public const string Base = "/api/services";
        public const string MediaItems = "/api/services/media-items";

        public static string ById(Guid id) => $"/api/services/{id}";
        public static string RemoveMediaItem(Guid id) => $"/api/services/media-items/{id}";
    }

    public static class Media
    {
        public const string Upload = "/api/media";
    }
}
