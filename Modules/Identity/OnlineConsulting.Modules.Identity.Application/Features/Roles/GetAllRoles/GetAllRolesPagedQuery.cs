using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Roles.GetAllRoles;

/// <summary>Paginated/sortable/filterable variant of GetAllRolesQuery - roles aren't tenant-scoped.</summary>
public record GetAllRolesPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<RoleResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [RolesOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, RolesOperationClaims.Read];
}

public class GetAllRolesPagedHandler(RoleManager<Role> roleManager) : IRequestHandler<GetAllRolesPagedQuery, OperationDataResult<Paginate<RoleResponse>>>
{
    public async Task<OperationDataResult<Paginate<RoleResponse>>> Handle(GetAllRolesPagedQuery request, CancellationToken cancellationToken)
    {
        var pagedRoles = await roleManager.Roles.ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: r => r.Name ?? string.Empty, cancellationToken);

        var items = pagedRoles.Items
            .Select(r => new RoleResponse { Id = r.Id, Name = r.Name ?? string.Empty, Description = r.Description })
            .ToList();

        return Result.Success(new Paginate<RoleResponse>
        {
            Items = items,
            Index = pagedRoles.Index,
            Size = pagedRoles.Size,
            Count = pagedRoles.Count,
            Pages = pagedRoles.Pages,
        }, items.Count == 0 ? RoleMessages.NoRoleDataFound : "Role data retrieved successfully.");
    }
}
