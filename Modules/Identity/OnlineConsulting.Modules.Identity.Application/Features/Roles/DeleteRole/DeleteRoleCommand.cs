using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.Rules;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Roles.DeleteRole;

public record DeleteRoleCommand(Guid RoleId) : IRequest<OperationResult>, ISecureAddRequest
{
    // Role isn't tenant-scoped (no TenantId) - only SuperAdmin may delete a role shared across tenants.
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];
}

public class DeleteRoleHandler(RoleManager<Role> roleManager) : IRequestHandler<DeleteRoleCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.RoleId.ToString());
        if (role is null)
        {
            return RoleBusinessRules.NoRoleDataFound();
        }

        var result = await roleManager.DeleteAsync(role);

        return result.Succeeded
            ? Result.Success("The role has been successfully deleted.")
            : Result.InternalServerError($"{string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"))} errors occurred while deleting the role. Please try again later.");
    }
}
