namespace OnlineConsulting.SharedKernel.Referrals;

/// <summary>Cross-module access to a user's account-credit ledger, so a purchase can reserve credit before charging and reverse it if the charge fails, without referencing Referrals' types.</summary>
public interface IAccountCreditLedger
{
    Task<decimal> GetBalanceAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Net amount currently debited for <paramref name="sourceId"/>; zero when nothing is, or after a reversal.</summary>
    Task<decimal> GetDebitedAsync(Guid userId, string sourceType, Guid sourceId, CancellationToken cancellationToken = default);

    /// <summary>Makes the net debit for <paramref name="sourceId"/> equal <paramref name="amount"/>, so repeating the call never debits twice. False, with nothing written, when the balance can't cover it.</summary>
    Task<bool> TryDebitAsync(Guid userId, decimal amount, string reason, string sourceType, Guid sourceId, CancellationToken cancellationToken = default);

    /// <summary>Credits back whatever is still debited for <paramref name="sourceId"/>; a no-op when nothing is.</summary>
    Task ReverseAsync(Guid userId, string reason, string sourceType, Guid sourceId, CancellationToken cancellationToken = default);
}
