using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace OnlineConsulting.SharedKernel.LiveUpdates;

/// <summary>Collects (user, topic) signals from the rule sets while SaveChanges runs and flushes them when no transaction is open.</summary>
public sealed class UserDataChangeSaveInterceptor(IEnumerable<UserDataChangeRuleSet> ruleSets, UserDataChangeBuffer buffer) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        await CollectAsync(eventData.Context, cancellationToken);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        CollectAsync(eventData.Context, CancellationToken.None).GetAwaiter().GetResult();
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context?.Database.CurrentTransaction is null)
        {
            await buffer.FlushAsync(eventData.Context, cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        if (eventData.Context?.Database.CurrentTransaction is null)
        {
            buffer.FlushAsync(eventData.Context, CancellationToken.None).GetAwaiter().GetResult();
        }

        return base.SavedChanges(eventData, result);
    }

    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        buffer.Discard(eventData.Context);
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        buffer.Discard(eventData.Context);
        base.SaveChangesFailed(eventData);
    }

    private async Task CollectAsync(DbContext? context, CancellationToken cancellationToken)
    {
        if (context is null)
        {
            return;
        }

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        List<UserDataChange> changes = [];
        foreach (var entry in entries)
        {
            foreach (var rule in ruleSets.SelectMany(set => set.RulesFor(entry.Entity.GetType())).Where(r => r.AppliesTo(entry)))
            {
                var userIds = await rule.ResolveUsers(entry, cancellationToken);
                changes.AddRange(userIds.Where(id => id != Guid.Empty).Select(id => new UserDataChange(id, rule.Topic)));
            }
        }

        if (changes.Count > 0)
        {
            buffer.Add(context, changes);
        }
    }
}
