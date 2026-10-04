using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using Core.SecurityLayer.Authorization;
using Core.SecurityLayer.Constants;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Application.Features.Auth;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.CurrentUser;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.ListUsers;

/// <summary>DynamicQuery carries filter+sort. Tenant scoping stays a separate .Where(), applied first. Role, when set, keeps only users holding it.</summary>
public record ListUsersQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null, string? Role = null) : IRequest<OperationDataResult<Paginate<UserResponse>>>, ISecureAddRequest, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(User.FirstName), nameof(User.LastName), nameof(User.Email), nameof(User.IsActive), nameof(User.CreatedDate)]);

    [JsonIgnore]
    public string[] Roles => [UsersOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, UsersOperationClaims.Read];
}

/// <summary>
/// Lists users, scoped to the caller's tenant unless the caller is Super Admin. A non-Super Admin caller
/// never sees a Super Admin account even if it shares their TenantId (e.g. invited directly by one) - see
/// <see cref="OnlineConsulting.Modules.Identity.Application.Common.TenantOwnerProtection"/>.
/// </summary>
public class ListUsersHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IPermissionCatalog permissionCatalog, ITenantProvider tenantProvider, ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<ListUsersQuery, OperationDataResult<Paginate<UserResponse>>>
{
    public async Task<OperationDataResult<Paginate<UserResponse>>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        var callerRoles = currentUserAccessor.Roles;
        var isSuperAdmin = callerRoles.Contains(GlobalOperationClaims.SuperAdmin);

        var usersQuery = isSuperAdmin
            ? userManager.Users
            : userManager.Users.Where(u => u.TenantId == tenantProvider.TenantId);

        if (!isSuperAdmin)
        {
            var superAdminIds = (await userManager.GetUsersInRoleAsync(GlobalOperationClaims.SuperAdmin)).Select(u => u.Id).ToHashSet();

            if (superAdminIds.Count > 0)
            {
                usersQuery = usersQuery.Where(u => !superAdminIds.Contains(u.Id));
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            var roleUserIds = (await userManager.GetUsersInRoleAsync(request.Role)).Select(u => u.Id).ToList();
            usersQuery = usersQuery.Where(u => roleUserIds.Contains(u.Id));
        }

        var pagedUsers = await usersQuery.ToDynamicPaginateAsync(request, defaultOrderBy: u => u.LastName, tieBreaker: u => u.Id, cancellationToken: cancellationToken);

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

        var (permissionsByRole, roleNamesByUserId) = await GetRoleDataAsync(userManager, roleManager, cancellationToken);

        var items = new List<UserResponse>();

        foreach (var user in pagedUsers.Items)
        {
            var roles = roleNamesByUserId[user.Id];
            var permissions = RolePermissionResolver.ExpandForDisplay([.. roles.SelectMany(role => permissionsByRole.GetValueOrDefault(role, [])).Distinct()], permissionCatalog);

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
                CreatedDate = user.CreatedDate,
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

    /// <summary>One pass over the (small, fixed) role set instead of one GetRolesAsync call per user on the page.</summary>
    private static async Task<(Dictionary<string, List<string>> PermissionsByRole, ILookup<Guid, string> RoleNamesByUserId)> GetRoleDataAsync(UserManager<User> userManager, RoleManager<Role> roleManager, CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.ToListAsync(cancellationToken);

        var permissionsByRole = new Dictionary<string, List<string>>();
        var userRolePairs = new List<(Guid UserId, string RoleName)>();

        foreach (var role in roles)
        {
            if (role.Name is null)
            {
                continue;
            }

            var claims = await roleManager.GetClaimsAsync(role);
            permissionsByRole[role.Name] = [.. claims.Where(c => c.Type == PermissionClaimTypes.Type).Select(c => c.Value)];

            userRolePairs.AddRange((await userManager.GetUsersInRoleAsync(role.Name)).Select(user => (user.Id, role.Name)));
        }

        return (permissionsByRole, userRolePairs.ToLookup(pair => pair.UserId, pair => pair.RoleName));
    }
}
