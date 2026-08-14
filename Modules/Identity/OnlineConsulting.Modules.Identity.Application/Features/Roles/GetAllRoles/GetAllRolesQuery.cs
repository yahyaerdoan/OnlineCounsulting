using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Roles.GetAllRoles;

public record GetAllRolesQuery : IRequest<OperationDataResult<List<RoleResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [RolesOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, RolesOperationClaims.Read];
}

public class GetAllRolesHandler(RoleManager<Role> roleManager) : IRequestHandler<GetAllRolesQuery, OperationDataResult<List<RoleResponse>>>
{
    public async Task<OperationDataResult<List<RoleResponse>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.AsNoTracking().ToListAsync(cancellationToken);
        if (roles.Count == 0)
            return Result.NotFound<List<RoleResponse>>("No role data found.");

        var roleResponses = roles.Select(r => new RoleResponse { Id = r.Id, Name = r.Name ?? string.Empty, Description = r.Description }).ToList();
        return Result.Success(roleResponses, "Role data retrieved successfully.");
    }
}
