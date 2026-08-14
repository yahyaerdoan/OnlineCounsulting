namespace OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;

public record RoleAssignmentResponse(Guid RoleId, string RoleName, bool IsAssigned);
