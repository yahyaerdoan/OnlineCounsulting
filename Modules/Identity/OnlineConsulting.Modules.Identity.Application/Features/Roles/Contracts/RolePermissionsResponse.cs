namespace OnlineConsulting.Modules.Identity.Application.Features.Roles.Contracts;

/// <summary>One role's assigned permissions - used to build the all-roles permission matrix.</summary>
public class RolePermissionsResponse
{
    public required Guid RoleId { get; init; }
    public required string RoleName { get; init; }
    public required List<string> Permissions { get; init; }
}
