namespace OnlineConsulting.SharedKernel.CurrentUser;

/// <summary>The caller of the current operation, read from the signed-in principal; empty for anonymous calls and background work, so handlers never touch HttpContext.</summary>
public interface ICurrentUserAccessor
{
    bool IsAuthenticated { get; }

    string? UserId { get; }

    /// <summary><see cref="UserId"/> as a Guid; null when anonymous.</summary>
    Guid? Id { get; }

    string? Email { get; }

    string? UserName { get; }

    IReadOnlyCollection<string> Roles { get; }

    IReadOnlyCollection<string> Permissions { get; }

    bool IsInRole(string role);

    bool HasPermission(string permission);
}
