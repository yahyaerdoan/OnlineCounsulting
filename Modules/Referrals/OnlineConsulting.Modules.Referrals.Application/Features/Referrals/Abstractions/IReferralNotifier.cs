using OnlineConsulting.Modules.Referrals.Domain;

namespace OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Abstractions;

/// <summary>Tells the referrer about their reward: an email and an in-app notification. Best effort: failures are logged, never thrown.</summary>
public interface IReferralNotifier
{
    Task RewardEarnedAsync(Referral referral, decimal amount, CancellationToken cancellationToken = default);
}
