using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Roles.GetRoleById;

public record GetRoleByIdQuery(Guid RoleId) : IRequest<OperationDataResult<RoleResponse>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [RolesOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, RolesOperationClaims.Read];
}

public class GetRoleByIdHandler(RoleManager<Role> roleManager) : IRequestHandler<GetRoleByIdQuery, OperationDataResult<RoleResponse>>
{
    public async Task<OperationDataResult<RoleResponse>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.RoleId.ToString());
        return role is null
            ? Result.NotFound<RoleResponse>(RoleMessages.NoRoleDataFound)
            : Result.Success(new RoleResponse { Id = role.Id, Name = role.Name ?? string.Empty, Description = role.Description }, "Role data retrieved successfully.");
    }
}
