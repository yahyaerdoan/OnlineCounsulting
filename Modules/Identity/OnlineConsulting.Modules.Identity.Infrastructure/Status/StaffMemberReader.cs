using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Identity;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Status;

/// <summary>Users is tenant-filtered, so other tenants' staff never count.</summary>
public class StaffMemberReader(AppIdentityDbContext context) : IStaffMemberReader
{
    public Task<bool> IsStaffAsync(Guid userId, CancellationToken cancellationToken = default) =>
        (from user in context.Users
         join userRole in context.UserRoles on user.Id equals userRole.UserId
         join role in context.Roles on userRole.RoleId equals role.Id
         where user.Id == userId && user.IsActive && role.Name != GlobalOperationClaims.User
         select user.Id)
            .AnyAsync(cancellationToken);
}
