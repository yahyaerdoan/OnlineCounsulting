namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/referrals/my-credit's response shape.</summary>
public record AccountCreditSummaryResponse(decimal Balance, List<AccountCreditResponse> Entries);

public record AccountCreditResponse(Guid Id, decimal Amount, string Reason, string SourceType, Guid SourceId);
