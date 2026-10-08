using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.PersistenceLayer.MultiTenancy;
using Core.SecurityLayer.Constants;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Abstractions;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Rules;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.CurrentUser;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.DeleteUser;

public record DeleteUserCommand(Guid UserId) : IRequest<OperationResult>, ISecureAddRequest
{
    public string[] Roles => [UsersOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, UsersOperationClaims.Delete];
}

public class DeleteUserHandler(UserManager<User> userManager, ITenantOwnershipReader tenantOwnershipReader, ITenantProvider tenantProvider, ICurrentUserAccessor currentUserAccessor, IUserRoleReader userRoleReader)
    : IRequestHandler<DeleteUserCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindManageableUserAsync(request.UserId, currentUserAccessor, cancellationToken);
        if (user is null)
        {
            return UserBusinessRules.NoUserDataFound();
        }

        if (currentUserAccessor.UserId == user.Id.ToString())
        {
            return Result.Forbidden("You cannot delete your own account.");
        }

        var ownerGuardResult = await TenantOwnerProtection.EnsureCallerMayModifyAsync(userManager, tenantOwnershipReader, tenantProvider, currentUserAccessor, user, cancellationToken);
        if (ownerGuardResult is not null)
        {
            return ownerGuardResult;
        }

        if (await userManager.IsInRoleAsync(user, GeneralOperationClaims.Admin))
        {
            if (!await userRoleReader.AnyOtherActiveUserInRoleAsync(GeneralOperationClaims.Admin, user.Id, user.TenantId, cancellationToken))
            {
                return Result.Forbidden("Cannot delete this user - this tenant would be left with no active admin.");
            }
        }

        using var tenantScope = TenantScope.Begin(user.TenantId);
        var result = await userManager.DeleteAsync(user);

        return result.Succeeded
            ? Result.Success("The user has been successfully deleted.")
            : Result.InternalServerError($"{string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"))} errors occurred while deleting the user. Please try again later.");
    }
}
