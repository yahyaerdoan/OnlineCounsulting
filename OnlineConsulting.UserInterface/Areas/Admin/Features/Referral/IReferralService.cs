using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Referral;

/// <summary>All Api orchestration for the admin Referrals oversight screen. ReferralResponse only carries raw
/// UserIds - referrer/referred names are resolved from a bulk GET /api/users lookup (same pattern as
/// AppointmentDispatchService/MembershipPlanService), no N+1.</summary>
public interface IReferralService
{
    Task<List<ReferralListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CompleteReferralViewModel?> GetCompleteFormAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CompleteAsync(Guid id, decimal rewardAmount, CancellationToken cancellationToken = default);
}
