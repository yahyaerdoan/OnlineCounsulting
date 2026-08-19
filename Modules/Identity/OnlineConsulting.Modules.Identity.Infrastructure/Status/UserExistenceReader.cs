using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Identity;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Status;

/// <summary>Cross-module implementation of IUserExistenceReader, backing Tenancy's orphaned-tenant cleanup job.</summary>
public class UserExistenceReader(AppIdentityDbContext context) : IUserExistenceReader
{
    public Task<bool> AnyUserExistsForTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        context.Users.AnyAsync(u => u.TenantId == tenantId && u.DeletedDate == null, cancellationToken);
}
