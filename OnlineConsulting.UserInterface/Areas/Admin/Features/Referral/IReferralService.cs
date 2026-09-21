using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Referral;

/// <summary>Referrals oversight orchestration; referrer/referred names are resolved via a bulk user lookup to avoid N+1.</summary>
public interface IReferralService
{
    /// <summary>Fetches all referrals with resolved referrer/referred names.</summary>
    Task<List<ReferralListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Fetches a referral's complete-reward form data, or null if not found.</summary>
    Task<CompleteReferralViewModel?> GetCompleteFormAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Marks a referral complete and records the reward amount.</summary>
    Task<ApiEnvelope> CompleteAsync(Guid id, decimal rewardAmount, CancellationToken cancellationToken = default);
}
