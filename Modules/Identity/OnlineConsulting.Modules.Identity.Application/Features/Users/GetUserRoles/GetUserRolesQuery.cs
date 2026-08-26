using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.GetUserRoles;

public record GetUserRolesQuery(Guid UserId) : IRequest<OperationDataResult<List<RoleAssignmentResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [UsersOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, UsersOperationClaims.Read];
}

public class GetUserRolesHandler(UserManager<User> userManager, RoleManager<Role> roleManager, ITenantProvider tenantProvider, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetUserRolesQuery, OperationDataResult<List<RoleAssignmentResponse>>>
{
    public async Task<OperationDataResult<List<RoleAssignmentResponse>>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            return Result.NotFound<List<RoleAssignmentResponse>>(UserMessages.UserNotFound);
        }

        if (!TenantOwnershipGuard.CallerMayManage(user.TenantId, tenantProvider.TenantId, httpContextAccessor))
        {
            return Result.Forbidden<List<RoleAssignmentResponse>>(UserMessages.NotAuthorizedForOtherTenant);
        }

        var allRoles = await roleManager.Roles.ToListAsync(cancellationToken);
        if (allRoles.Count == 0)
        {
            return Result.NotFound<List<RoleAssignmentResponse>>("Roles not found.");
        }

        var userRoleNames = await userManager.GetRolesAsync(user);

        var assignments = allRoles
            .Select(role => new RoleAssignmentResponse(role.Id, role.Name ?? string.Empty, userRoleNames.Contains(role.Name ?? string.Empty)))
            .ToList();

        return Result.Success(assignments, "User roles retrieved successfully.");
    }
}
