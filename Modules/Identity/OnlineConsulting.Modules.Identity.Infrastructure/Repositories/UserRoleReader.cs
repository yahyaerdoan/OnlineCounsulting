using Core.PersistenceLayer.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Abstractions;
using OnlineConsulting.Modules.Identity.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Repositories;

public class UserRoleReader(AppIdentityDbContext context) : IUserRoleReader
{
    public async Task<IReadOnlyList<Guid>> GetUserIdsInRoleAsync(string roleName, CancellationToken cancellationToken = default) =>
        await (from userRole in context.UserRoles
               join role in context.Roles on userRole.RoleId equals role.Id
               where role.Name == roleName
               select userRole.UserId)
            .ToListAsync(cancellationToken);

    public async Task<ILookup<Guid, string>> GetRoleNamesAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken = default)
    {
        var assignments = await (from userRole in context.UserRoles
                                 join role in context.Roles on userRole.RoleId equals role.Id
                                 where userIds.Contains(userRole.UserId) && role.Name != null
                                 select new { userRole.UserId, RoleName = role.Name ?? string.Empty })
            .ToListAsync(cancellationToken);

        return assignments.ToLookup(assignment => assignment.UserId, assignment => assignment.RoleName);
    }

    public Task<bool> AnyOtherActiveUserInRoleAsync(string roleName, Guid excludedUserId, Guid? tenantId, CancellationToken cancellationToken = default) =>
        (from userRole in context.UserRoles
         join role in context.Roles on userRole.RoleId equals role.Id
         join user in context.Users.IgnoreTenantFilter() on userRole.UserId equals user.Id
         where role.Name == roleName && user.Id != excludedUserId && user.IsActive && (tenantId == null || user.TenantId == tenantId)
         select user.Id)
            .AnyAsync(cancellationToken);
}
