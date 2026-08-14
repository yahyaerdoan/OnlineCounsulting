using Hateoas;

namespace OnlineConsulting.Modules.Identity.Application.Features.Roles.Contracts;

public class RoleResponse : LinkedResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}
