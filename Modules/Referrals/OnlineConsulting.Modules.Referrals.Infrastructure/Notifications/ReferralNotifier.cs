using System.Globalization;
using Microsoft.Extensions.Logging;
using OnlineConsulting.Modules.Referrals.Application.Common.Templates;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Abstractions;
using OnlineConsulting.Modules.Referrals.Domain;
using OnlineConsulting.SharedKernel.Identity;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;

namespace OnlineConsulting.Modules.Referrals.Infrastructure.Notifications;

/// <summary>Emails go through the Referrals outbox; the notification carries "referralId" so tapping it opens the referrals page.</summary>
public class ReferralNotifier(
    IEmailOutboxWriter<IReferralsOutboxModule> outboxWriter,
    IEmailTemplate<ReferralRewardEmailModel> rewardTemplate,
    IPushNotificationSender pushSender,
    IUserContactReader contactReader,
    ILogger<ReferralNotifier> logger) : IReferralNotifier
{
    public async Task RewardEarnedAsync(Referral referral, decimal amount, CancellationToken cancellationToken = default)
    {
        try
        {
            if (await contactReader.GetContactAsync(referral.ReferrerUserId, cancellationToken) is { Email: { Length: > 0 } email } contact)
            {
                var model = new ReferralRewardEmailModel(contact.FirstName, amount);
                await outboxWriter.EnqueueAsync(email, rewardTemplate.Subject(model), rewardTemplate.Build(model), sourceReference: $"Referral:{referral.Id}:Rewarded",
                    cancellationToken: cancellationToken);
            }
            else
            {
                logger.LogWarning("Referral {ReferralId}: no email on file for referrer {UserId}, skipping the reward email.", referral.Id, referral.ReferrerUserId);
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Referral {ReferralId}: queuing the reward email failed.", referral.Id);
        }

        try
        {
            await pushSender.SendToUserAsync(referral.ReferrerUserId, "Referral reward earned",
                $"You've earned {amount.ToString("C", CultureInfo.GetCultureInfo("en-US"))} in account credit for your referral!",
                new Dictionary<string, string> { ["referralId"] = referral.Id.ToString() }, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Referral {ReferralId}: sending the reward notification failed.", referral.Id);
        }
    }
}
