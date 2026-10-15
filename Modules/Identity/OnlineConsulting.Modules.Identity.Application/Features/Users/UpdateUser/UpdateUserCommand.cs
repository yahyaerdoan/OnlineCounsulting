using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.PersistenceLayer.MultiTenancy;
using Core.SecurityLayer.Constants;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Abstractions;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.CurrentUser;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.UpdateUser;

public record UpdateUserCommand(Guid Id, string FirstName, string LastName, bool IsActive) : IRequest<OperationResult>, ISecureAddRequest
{
    public string[] Roles => [UsersOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, UsersOperationClaims.Update];
}

public class UpdateUserHandler(UserManager<User> userManager, ITenantOwnershipReader tenantOwnershipReader, ITenantProvider tenantProvider, ICurrentUserAccessor currentUserAccessor, IUserRoleReader userRoleReader)
    : IRequestHandler<UpdateUserCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindManageableUserAsync(request.Id, currentUserAccessor, cancellationToken);
        if (user is null)
        {
            return Result.NotFound(UserMessages.UserNotFound);
        }

        if (!request.IsActive && currentUserAccessor.UserId == user.Id.ToString())
        {
            return Result.Forbidden("You cannot deactivate your own account.");
        }

        var ownerGuardResult = await TenantOwnerProtection.EnsureCallerMayModifyAsync(userManager, tenantOwnershipReader, tenantProvider, currentUserAccessor, user, cancellationToken);
        if (ownerGuardResult is not null)
        {
            return ownerGuardResult;
        }

        if (!request.IsActive && user.IsActive && await userManager.IsInRoleAsync(user, GeneralOperationClaims.Admin))
        {
            if (!await userRoleReader.AnyOtherActiveUserInRoleAsync(GeneralOperationClaims.Admin, user.Id, user.TenantId, cancellationToken))
            {
                return Result.Forbidden("Cannot deactivate this user - this tenant would be left with no active admin.");
            }
        }

        using var tenantScope = TenantScope.Begin(user.TenantId);

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.IsActive = request.IsActive;

        var result = await userManager.UpdateAsync(user);

        return result.Succeeded
            ? Result.Success("The user has been successfully updated.")
            : Result.InternalServerError("An error occurred while updating the user. Please try again later.");
    }
}
