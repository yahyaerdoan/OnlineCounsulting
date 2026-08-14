using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.Constants;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Roles.UpdateRole;

public record UpdateRoleCommand(Guid Id, string Name, string? Description) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [RolesOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, RolesOperationClaims.Update];
}

public class UpdateRoleHandler(RoleManager<Role> roleManager) : IRequestHandler<UpdateRoleCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());
        if (role is null)
            return Result.BadRequest("The role could not be found. Please ensure the provided data is correct and try again.");

        role.Name = request.Name;
        role.Description = request.Description;

        var result = await roleManager.UpdateAsync(role);

        return result.Succeeded
            ? Result.Success("The role has been successfully updated.")
            : Result.BadRequest($"{string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"))} errors occurred while saving the role. Please try again later.");
    }
}
