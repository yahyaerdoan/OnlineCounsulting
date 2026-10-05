using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace OnlineConsulting.SharedKernel.LiveUpdates;

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
