using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Rules;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
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

public class DeleteUserHandler(UserManager<User> userManager, ITenantOwnershipReader tenantOwnershipReader, ITenantProvider tenantProvider, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<DeleteUserCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            return UserBusinessRules.NoUserDataFound();
        }

        var ownerGuardResult = await TenantOwnerProtection.EnsureCallerMayModifyAsync(tenantOwnershipReader, tenantProvider, httpContextAccessor, user.TenantId, user.Id, cancellationToken);
        if (ownerGuardResult is not null)
        {
            return ownerGuardResult;
        }

        var result = await userManager.DeleteAsync(user);

        return result.Succeeded
            ? Result.Success("The user has been successfully deleted.")
            : Result.InternalServerError($"{string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"))} errors occurred while deleting the user. Please try again later.");
    }
}
