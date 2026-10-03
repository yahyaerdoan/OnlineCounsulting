using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace OnlineConsulting.SharedKernel.LiveUpdates;

/// <summary>Per-request holding area: changes are collected while saving and only published once they are durable - right
/// after SaveChanges when there is no transaction, otherwise when the surrounding transaction commits. Publishing
/// earlier would let a client refetch before the commit and see the old data.</summary>
public sealed class UserDataChangeBuffer(IUserDataChangePublisher publisher, ILogger<UserDataChangeBuffer> logger)
{
    private readonly Dictionary<DbContext, HashSet<UserDataChange>> _pending = [];
    private readonly Lock _gate = new();

    public void Add(DbContext context, IEnumerable<UserDataChange> changes)
    {
        lock (_gate)
        {
            if (!_pending.TryGetValue(context, out var set))
            {
                set = [];
                _pending[context] = set;
            }

            set.UnionWith(changes);
        }
    }

    public void Discard(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        lock (_gate)
        {
            _ = _pending.Remove(context);
        }
    }

    /// <summary>Publishes and clears what this context collected; a transport failure is logged, never thrown - the data is
    /// already committed and clients still catch up on their next resume/refresh.</summary>
    public async Task FlushAsync(DbContext? context, CancellationToken cancellationToken)
    {
        if (context is null)
        {
            return;
        }

        UserDataChange[] changes;
        lock (_gate)
        {
            if (!_pending.Remove(context, out var set) || set.Count == 0)
            {
                return;
            }

            changes = [.. set];
        }

        try
        {
            await publisher.PublishAsync(changes, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogWarning(exception, "Publishing {Count} user data change signal(s) failed.", changes.Length);
        }
    }
}

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

/// <summary>Publishes the buffered signals when the context's transaction commits and drops them on rollback.</summary>
public sealed class UserDataChangeTransactionInterceptor(UserDataChangeBuffer buffer) : DbTransactionInterceptor
{
    public override async Task TransactionCommittedAsync(DbTransaction transaction, TransactionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        await buffer.FlushAsync(eventData.Context, cancellationToken);
        await base.TransactionCommittedAsync(transaction, eventData, cancellationToken);
    }

    public override void TransactionCommitted(DbTransaction transaction, TransactionEndEventData eventData)
    {
        buffer.FlushAsync(eventData.Context, CancellationToken.None).GetAwaiter().GetResult();
        base.TransactionCommitted(transaction, eventData);
    }

    public override Task TransactionRolledBackAsync(DbTransaction transaction, TransactionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        buffer.Discard(eventData.Context);
        return base.TransactionRolledBackAsync(transaction, eventData, cancellationToken);
    }

    public override void TransactionRolledBack(DbTransaction transaction, TransactionEndEventData eventData)
    {
        buffer.Discard(eventData.Context);
        base.TransactionRolledBack(transaction, eventData);
    }

    public override Task TransactionFailedAsync(DbTransaction transaction, TransactionErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        buffer.Discard(eventData.Context);
        return base.TransactionFailedAsync(transaction, eventData, cancellationToken);
    }

    public override void TransactionFailed(DbTransaction transaction, TransactionErrorEventData eventData)
    {
        buffer.Discard(eventData.Context);
        base.TransactionFailed(transaction, eventData);
    }
}
