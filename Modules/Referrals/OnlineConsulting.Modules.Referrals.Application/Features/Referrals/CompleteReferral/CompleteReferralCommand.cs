using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Referrals.Application.Common;
using OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.Abstractions;
using OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.Constants;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Abstractions;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Constants;
using OnlineConsulting.Modules.Referrals.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Referrals.Application.Features.Referrals.CompleteReferral;

/// <summary>Manual admin action, not automated - rewarding a referral is a real payout, so it stays a deliberate confirmation step rather than firing automatically off some app event (same reasoning as ConfirmAppointment staying a manual tenant decision). Two writes (Referral update + AccountCredit add), hence ITransactionAddRequest.</summary>
public record CompleteReferralCommand(Guid Id, decimal RewardAmount) : IRequest<OperationResult>, ISecureAddRequest, ITransactionAddRequest
{
    [JsonIgnore]
    public string[] Roles => [ReferralsOperationClaims.Admin, ReferralsOperationClaims.Write];
}

public class CompleteReferralHandler(IReferralRepository referralRepository, IAccountCreditRepository creditRepository, IPushNotificationSender pushNotificationSender)
    : IRequestHandler<CompleteReferralCommand, OperationResult>
{
    public async Task<OperationResult> Handle(CompleteReferralCommand request, CancellationToken cancellationToken)
    {
        var referral = await referralRepository.GetAsync(r => r.Id == request.Id, cancellationToken: cancellationToken);
        if (referral is null)
            return Result.NotFound(string.Format(ReferralsMessages.ReferralNotFoundFormat, request.Id));

        if (referral.Status == ReferralStatuses.Rewarded)
            return Result.BadRequest(ReferralsMessages.AlreadyRewarded);

        referral.Status = ReferralStatuses.Rewarded;
        referral.RewardAmount = request.RewardAmount;
        referral.RewardedAt = DateTimeOffset.UtcNow;

        await referralRepository.UpdateAsync(referral);

        await creditRepository.AddAsync(new AccountCredit
        {
            Id = Guid.NewGuid(),
            UserId = referral.ReferrerUserId,
            Amount = request.RewardAmount,
            Reason = "Referral reward",
            SourceType = AccountCreditSourceTypes.Referral,
            SourceId = referral.Id,
        });

        await pushNotificationSender.SendToUserAsync(
            referral.ReferrerUserId,
            "Referral reward earned",
            $"You've earned ${request.RewardAmount:0.##} in account credit for your referral!",
            new Dictionary<string, string> { ["referralId"] = referral.Id.ToString() },
            cancellationToken);

        return Result.Success("Referral rewarded successfully.");
    }
}
