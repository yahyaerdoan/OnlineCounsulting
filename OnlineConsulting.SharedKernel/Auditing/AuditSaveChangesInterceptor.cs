using Core.PersistenceLayer.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OnlineConsulting.SharedKernel.CurrentUser;

namespace OnlineConsulting.SharedKernel.Auditing;

public class AuditSaveChangesInterceptor(ICurrentUserAccessor currentUserAccessor) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Stamp(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Stamp(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Stamp(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var userId = currentUserAccessor.UserId;
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<IEntityAuditor>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = now;
                entry.Entity.CreatedBy = userId;
                continue;
            }

            if (entry.State != EntityState.Modified)
            {
                continue;
            }

            var isSoftDelete = entry.Property(nameof(IEntityAuditor.DeletedDate)).IsModified && entry.Entity.DeletedDate.HasValue;
            if (isSoftDelete)
            {
                entry.Entity.DeletedBy = userId;
                continue;
            }

            entry.Entity.UpdatedDate = now;
            entry.Entity.UpdatedBy = userId;
        }
    }
}
