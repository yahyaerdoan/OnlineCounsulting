namespace OnlineConsulting.Modules.Identity.Application.Features.Roles.Contracts;

public class RoleResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}
