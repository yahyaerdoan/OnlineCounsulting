using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Referrals.Domain;

/// <summary>Append-only ledger entry - balance is always the sum of entries, never a mutable counter, so it can't drift from its history.</summary>
public class AccountCredit : SequentialGuidTenantEntity
{
    public required Guid UserId { get; set; }
    public required decimal Amount { get; set; }
    public required string Reason { get; set; }
    public required string SourceType { get; set; }
    public required Guid SourceId { get; set; }
}
