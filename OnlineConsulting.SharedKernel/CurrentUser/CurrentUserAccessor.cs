using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.SecurityLayer.Extensions;
using Microsoft.AspNetCore.Http;

namespace OnlineConsulting.SharedKernel.CurrentUser;

public class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public Guid? Id => Guid.TryParse(UserId, out var id) ? id : null;

    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value ?? User?.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

    public string? UserName => User?.Identity?.Name ?? User?.FindFirst(ClaimTypes.Name)?.Value;

    public IReadOnlyCollection<string> Roles => User?.ClaimRoles() ?? [];

    public IReadOnlyCollection<string> Permissions => User?.ClaimPermissions() ?? [];

    public bool IsInRole(string role) => Roles.Contains(role);

    public bool HasPermission(string permission) => Permissions.Contains(permission);
}
