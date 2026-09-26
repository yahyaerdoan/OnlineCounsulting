using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Referrals.Domain;
using OnlineConsulting.Modules.Referrals.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Referrals;
using System.Data;

namespace OnlineConsulting.Modules.Referrals.Infrastructure.Ledger;

/// <summary>Cross-module implementation of IAccountCreditLedger; every write is an append-only adjusting entry, run under a serializable transaction so two concurrent debits can't both pass the balance check.</summary>
public class AccountCreditLedger(ReferralsDbContext context) : IAccountCreditLedger
{
    public Task<decimal> GetBalanceAsync(Guid userId, CancellationToken cancellationToken = default)
        => context.AccountCredits.Where(c => c.UserId == userId).SumAsync(c => c.Amount, cancellationToken);

    public async Task<decimal> GetDebitedAsync(Guid userId, string sourceType, Guid sourceId, CancellationToken cancellationToken = default)
        => -await context.AccountCredits
            .Where(c => c.UserId == userId && c.SourceType == sourceType && c.SourceId == sourceId)
            .SumAsync(c => c.Amount, cancellationToken);

    public async Task<bool> TryDebitAsync(Guid userId, decimal amount, string reason, string sourceType, Guid sourceId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        var debited = await GetDebitedAsync(userId, sourceType, sourceId, cancellationToken);
        if (debited == amount)
        {
            return true;
        }

        var available = await GetBalanceAsync(userId, cancellationToken) + debited;
        if (amount > available)
        {
            return false;
        }

        await AppendAsync(userId, debited - amount, reason, sourceType, sourceId, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    public async Task ReverseAsync(Guid userId, string reason, string sourceType, Guid sourceId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        var debited = await GetDebitedAsync(userId, sourceType, sourceId, cancellationToken);
        if (debited <= 0)
        {
            return;
        }

        await AppendAsync(userId, debited, reason, sourceType, sourceId, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task AppendAsync(Guid userId, decimal amount, string reason, string sourceType, Guid sourceId, CancellationToken cancellationToken)
    {
        _ = context.AccountCredits.Add(new AccountCredit
        {
            UserId = userId,
            Amount = amount,
            Reason = reason,
            SourceType = sourceType,
            SourceId = sourceId,
        });

        _ = await context.SaveChangesAsync(cancellationToken);
    }
}
