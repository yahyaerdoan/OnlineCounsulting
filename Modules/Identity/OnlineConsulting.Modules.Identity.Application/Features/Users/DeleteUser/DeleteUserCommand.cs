using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Rules;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.CurrentUser;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.DeleteUser;

public record DeleteUserCommand(Guid UserId) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [UsersOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, UsersOperationClaims.Delete];
}

public class DeleteUserHandler(UserManager<User> userManager, ITenantOwnershipReader tenantOwnershipReader, ITenantProvider tenantProvider, IHttpContextAccessor httpContextAccessor, ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<DeleteUserCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            return UserBusinessRules.NoUserDataFound();
        }

        if (currentUserAccessor.UserId == user.Id.ToString())
        {
            return Result.Forbidden("You cannot delete your own account.");
        }

        var ownerGuardResult = await TenantOwnerProtection.EnsureCallerMayModifyAsync(userManager, tenantOwnershipReader, tenantProvider, httpContextAccessor, user, cancellationToken);
        if (ownerGuardResult is not null)
        {
            return ownerGuardResult;
        }

        if (await userManager.IsInRoleAsync(user, GeneralOperationClaims.Admin))
        {
            var tenantAdmins = await userManager.GetUsersInRoleAsync(GeneralOperationClaims.Admin);
            var hasOtherActiveAdmin = tenantAdmins.Any(a => a.Id != user.Id && a.TenantId == user.TenantId && a.IsActive);
            if (!hasOtherActiveAdmin)
            {
                return Result.Forbidden("Cannot delete this user - this tenant would be left with no active admin.");
            }
        }

        var result = await userManager.DeleteAsync(user);

        return result.Succeeded
            ? Result.Success("The user has been successfully deleted.")
            : Result.InternalServerError($"{string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"))} errors occurred while deleting the user. Please try again later.");
    }
}
