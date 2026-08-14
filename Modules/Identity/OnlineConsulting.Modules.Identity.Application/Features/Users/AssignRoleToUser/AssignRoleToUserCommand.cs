using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Rules;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.AssignRoleToUser;

public record AssignRoleToUserCommand(Guid UserId, List<RoleAssignmentRequest> RoleAssignments) : IRequest<OperationResult>, ISecureAddRequest, ITransactionAddRequest
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
            return UserBusinessRules.UserNotFoundOrInvalidData();

        foreach (var assignment in request.RoleAssignments)
        {
            var result = assignment.IsAssigned
                ? await userManager.AddToRoleAsync(user, assignment.RoleName)
                : await userManager.RemoveFromRoleAsync(user, assignment.RoleName);

            if (!result.Succeeded)
                return Result.BadRequest($"{string.Join("; ", result.Errors.Select(e => e.Description))} errors occurred while updating role \"{assignment.RoleName}\".");
        }

        return Result.Success("New permissions added successfully.");
    }
}
