using Hateoas;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;

public class UserResponse : LinkedResponse
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required string UserName { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public string? ImageUrl { get; init; }
    public required IReadOnlyList<string> Roles { get; init; }
    public IReadOnlyList<string> Permissions { get; init; } = [];
}
