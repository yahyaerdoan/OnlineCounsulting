using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.AssignRoleToUser;

public record AssignRoleToUserCommand(Guid UserId, List<RoleAssignmentRequest> RoleAssignments) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [UsersOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, UsersOperationClaims.Write];
}

public class AssignRoleToUserHandler(UserManager<User> userManager) : IRequestHandler<AssignRoleToUserCommand, OperationResult>
{
    public async Task<OperationResult> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return Result.BadRequest("The user could not be found. Please ensure the provided data is correct and try again.");

        foreach (var assignment in request.RoleAssignments)
        {
            if (assignment.IsAssigned)
                await userManager.AddToRoleAsync(user, assignment.RoleName);
            else
                await userManager.RemoveFromRoleAsync(user, assignment.RoleName);
        }

        return Result.Success("New permissions added successfully.");
    }
}
