using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OnlineConsulting.SharedKernel.LiveUpdates;

/// <summary>Per-request holding area: changes are collected while saving and only published once they are durable - right
/// after SaveChanges when there is no transaction, otherwise when the surrounding transaction commits. Publishing
/// earlier would let a client refetch before the commit and see the old data.</summary>
public sealed class UserDataChangeBuffer(IUserDataChangePublisher publisher, ILogger<UserDataChangeBuffer> logger)
{
    private readonly Dictionary<DbContext, HashSet<UserDataChange>> _pending = [];
    private readonly Lock _gate = new();

    /// <summary>Holds the changes until the context's work is durable.</summary>
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

    /// <summary>Drops the context's held changes, e.g. after a rollback.</summary>
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
