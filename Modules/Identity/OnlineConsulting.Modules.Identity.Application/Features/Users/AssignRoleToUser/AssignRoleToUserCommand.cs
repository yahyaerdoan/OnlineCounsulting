using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Loggings.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.SecurityLayer.Constants;
using Core.SecurityLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Rules;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.AssignRoleToUser;

public record AssignRoleToUserCommand(Guid UserId, List<RoleAssignmentRequest> RoleAssignments) : IRequest<OperationResult>, ISecureAddRequest, ITransactionAddRequest, ILogResultRequest
{
    [JsonIgnore]
    public string[] Roles => [UsersOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, UsersOperationClaims.Write];
}

public class AssignRoleToUserHandler(UserManager<User> userManager, ITenantOwnershipReader tenantOwnershipReader, ITenantProvider tenantProvider, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<AssignRoleToUserCommand, OperationResult>
{
    public async Task<OperationResult> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            return UserBusinessRules.UserNotFoundOrInvalidData();
        }

        var ownerGuardResult = await TenantOwnerProtection.EnsureCallerMayModifyAsync(userManager, tenantOwnershipReader, tenantProvider, httpContextAccessor, user, cancellationToken);
        if (ownerGuardResult is not null)
        {
            return ownerGuardResult;
        }

        var callerRoles = httpContextAccessor.HttpContext?.User.ClaimRoles() ?? [];
        var superAdminAssignment = request.RoleAssignments.FirstOrDefault(a => a.RoleName == GlobalOperationClaims.SuperAdmin);
        if (superAdminAssignment is not null && !callerRoles.Contains(GlobalOperationClaims.SuperAdmin))
        {
            // Callers resend the whole role matrix (see UserRolesDialog.razor), so an unchanged
            // SuperAdmin=false entry must pass - only reject an actual grant/revoke attempt.
            var currentlySuperAdmin = await userManager.IsInRoleAsync(user, GlobalOperationClaims.SuperAdmin);
            if (currentlySuperAdmin != superAdminAssignment.IsAssigned)
            {
                return Result.Forbidden("Only a Super Admin may grant or revoke the Super Admin role.");
            }
        }

        var adminAssignment = request.RoleAssignments.FirstOrDefault(a => a.RoleName == GeneralOperationClaims.Admin);
        if (adminAssignment is { IsAssigned: false } && await userManager.IsInRoleAsync(user, GeneralOperationClaims.Admin))
        {
            var tenantAdmins = await userManager.GetUsersInRoleAsync(GeneralOperationClaims.Admin);
            var hasOtherActiveAdmin = tenantAdmins.Any(a => a.Id != user.Id && a.TenantId == user.TenantId && a.IsActive);
            if (!hasOtherActiveAdmin)
            {
                return Result.Forbidden("Cannot remove the Admin role - this tenant would be left with no active admin.");
            }
        }

        foreach (var assignment in request.RoleAssignments)
        {
            if (await userManager.IsInRoleAsync(user, assignment.RoleName) == assignment.IsAssigned)
            {
                continue;
            }

            var result = assignment.IsAssigned
                ? await userManager.AddToRoleAsync(user, assignment.RoleName)
                : await userManager.RemoveFromRoleAsync(user, assignment.RoleName);

            if (!result.Succeeded)
            {
                return Result.BadRequest($"{string.Join("; ", result.Errors.Select(e => e.Description))} errors occurred while updating role \"{assignment.RoleName}\".");
            }
        }

        return Result.Success("New permissions added successfully.");
    }
}
