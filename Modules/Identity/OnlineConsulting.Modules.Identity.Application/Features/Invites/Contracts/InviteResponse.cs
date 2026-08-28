using Hateoas;

namespace OnlineConsulting.Modules.Identity.Application.Features.Invites.Contracts;

public class InviteResponse : LinkedResponse
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string RoleName { get; init; }
    public required string Status { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public required DateTimeOffset CreatedDate { get; init; }
}
