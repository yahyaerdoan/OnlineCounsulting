namespace OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;

public record RoleAssignmentRequest(Guid RoleId, string RoleName, bool IsAssigned);
