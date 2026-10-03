namespace OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.Contracts;

public record AccountCreditSummaryResponse(decimal Balance, List<AccountCreditResponse> Entries);
