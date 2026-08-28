using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using Core.SecurityLayer.Authorization;
using Core.SecurityLayer.Constants;
using Core.SecurityLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Application.Features.Auth;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.GetAllUsers;

/// <summary>DynamicQuery carries filter+sort. Tenant scoping stays a separate .Where(), applied first.</summary>
public record GetAllUsersQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null)
    : IRequest<OperationDataResult<Paginate<UserResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [UsersOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, UsersOperationClaims.Read];
}

public class GetAllUsersHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IPermissionCatalog permissionCatalog, ITenantProvider tenantProvider, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetAllUsersQuery, OperationDataResult<Paginate<UserResponse>>>
{
    public async Task<OperationDataResult<Paginate<UserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var callerRoles = httpContextAccessor.HttpContext?.User.ClaimRoles() ?? [];
        var isSuperAdmin = callerRoles.Contains(GlobalOperationClaims.SuperAdmin);

        var usersQuery = isSuperAdmin
            ? userManager.Users
            : userManager.Users.Where(u => u.TenantId == tenantProvider.TenantId);

        if (!isSuperAdmin)
        {
            // A non-SuperAdmin can share TenantId with a SuperAdmin (e.g. invited directly by one) -
            // never reveal that account to anyone who can't act on it (see TenantOwnerProtection).
            var superAdminIds = (await userManager.GetUsersInRoleAsync(GlobalOperationClaims.SuperAdmin)).Select(u => u.Id).ToHashSet();
            if (superAdminIds.Count > 0)
            {
                usersQuery = usersQuery.Where(u => !superAdminIds.Contains(u.Id));
            }
        }

        var pagedUsers = await usersQuery.ToDynamicPaginateAsync(
            request.PageRequest, request.DynamicQuery, defaultOrderBy: u => u.LastName, cancellationToken);

        if (pagedUsers.Items.Count == 0)
        {
            return Result.Success(new Paginate<UserResponse>
            {
                Items = [],
                Index = pagedUsers.Index,
                Size = pagedUsers.Size,
                Count = pagedUsers.Count,
                Pages = pagedUsers.Pages,
            }, UserMessages.NoUserDataFound);
        }

        var permissionsByRole = await GetPermissionsByRoleAsync(roleManager, cancellationToken);

        var items = new List<UserResponse>();
        foreach (var user in pagedUsers.Items)
        {
            var roles = await userManager.GetRolesAsync(user);
            var permissions = RolePermissionResolver.ExpandForDisplay(
                [.. roles.SelectMany(role => permissionsByRole.GetValueOrDefault(role, [])).Distinct()], permissionCatalog);

            items.Add(new UserResponse
            {
                Id = user.Id,
                TenantId = user.TenantId,
                UserName = user.UserName ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                ImageUrl = user.ImageUrl,
                IsActive = user.IsActive,
                Roles = [.. roles],
                Permissions = permissions,
            });
        }

        return Result.Success(new Paginate<UserResponse>
        {
            Items = items,
            Index = pagedUsers.Index,
            Size = pagedUsers.Size,
            Count = pagedUsers.Count,
            Pages = pagedUsers.Pages,
        }, "User data retrieved successfully.");
    }

    private static async Task<Dictionary<string, List<string>>> GetPermissionsByRoleAsync(RoleManager<Role> roleManager, CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.ToListAsync(cancellationToken);
        var permissionsByRole = new Dictionary<string, List<string>>();

        foreach (var role in roles)
        {
            if (role.Name is null)
            {
                continue;
            }

            var claims = await roleManager.GetClaimsAsync(role);
            permissionsByRole[role.Name] = [.. claims.Where(c => c.Type == PermissionClaimTypes.Type).Select(c => c.Value)];
        }

        return permissionsByRole;
    }
}
