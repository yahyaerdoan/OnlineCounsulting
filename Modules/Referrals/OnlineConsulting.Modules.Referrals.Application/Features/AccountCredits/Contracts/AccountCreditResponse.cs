using OnlineConsulting.Modules.Referrals.Domain;

namespace OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.Contracts;

public record AccountCreditResponse(Guid Id, decimal Amount, string Reason, string SourceType, Guid SourceId)
{
    public static AccountCreditResponse FromDomain(AccountCredit entity) => new(entity.Id, entity.Amount, entity.Reason, entity.SourceType, entity.SourceId);
}
