using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.SecurityLayer.Constants;
using Core.SecurityLayer.Extensions;

namespace OnlineConsulting.Api.Common.Hateoas;

/// <summary>
/// Answers "would this caller be allowed to send TRequest?" with the same rule as AuthorizationAddingBehavior, reading the request's
/// own Roles, so an action link is offered exactly when the action would not be refused and the two can never drift apart.
/// </summary>
public static class LinkPermissions
{
    private static readonly ConcurrentDictionary<Type, ISecureAddRequest> Requests = new();

    /// <summary>True when <paramref name="user"/> passes TRequest's authorization (authenticated, and holding one of its roles when it lists any).</summary>
    public static bool CanSend<TRequest>(this ClaimsPrincipal user)
        where TRequest : ISecureAddRequest
    {
        if (user.Identity?.IsAuthenticated is not true)
        {
            return false;
        }

        var request = Requests.GetOrAdd(typeof(TRequest), type => (ISecureAddRequest)RuntimeHelpers.GetUninitializedObject(type));
        if (request.Roles.Length == 0)
        {
            return true;
        }

        var roles = user.ClaimRoles() ?? [];
        var permissions = user.ClaimPermissions() ?? [];
        return permissions.Contains(PermissionClaimTypes.FullAccess)
            || (request.AllowTenantBypass && permissions.Contains(PermissionClaimTypes.TenantFullAccess))
            || request.Roles.Any(role => roles.Contains(role) || permissions.Contains(role));
    }

    /// <summary>True when the signed-in caller is the user <paramref name="userId"/> (owner-only actions such as paying one's own invoice).</summary>
    public static bool IsUser(this ClaimsPrincipal user, Guid userId)
        => Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) && id == userId;
}
