using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Referrals.Domain;

/// <summary>An append-only ledger entry, never updated/deleted once written - a user's balance is always the sum of their entries, not a mutable counter, so it can never drift from its own history. SourceType/SourceId trace back to whatever earned the credit (currently only "Referral"/Referral.Id).</summary>
public class AccountCredit : TenantEntity<Guid>
{
    public required Guid UserId { get; set; }
    public required decimal Amount { get; set; }
    public required string Reason { get; set; }
    public required string SourceType { get; set; }
    public required Guid SourceId { get; set; }
}
