using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Referrals.Application.Common;
using OnlineConsulting.Modules.Referrals.Application.Features.ReferralCodes.Abstractions;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Abstractions;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Constants;
using OnlineConsulting.Modules.Referrals.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Referrals.Application.Features.Referrals.RedeemReferralCode;

/// <summary>ReferredUserId is always resolved server-side from the authenticated caller, never trusted from the client. A user can redeem at most one referral code, ever - enforced here, not just at signup, so the same rule applies regardless of when the customer first hears about the program.</summary>
public record RedeemReferralCodeCommand(Guid ReferredUserId, string Code) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class RedeemReferralCodeHandler(IReferralRepository referralRepository, IReferralCodeRepository referralCodeRepository)
    : IRequestHandler<RedeemReferralCodeCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(RedeemReferralCodeCommand request, CancellationToken cancellationToken)
    {
        var referralCode = await referralCodeRepository.GetAsync(c => c.Code == request.Code, cancellationToken: cancellationToken);
        if (referralCode is null)
            return Result.NotFound<Guid>(ReferralsMessages.InvalidCode);

        if (referralCode.UserId == request.ReferredUserId)
            return Result.BadRequest<Guid>(ReferralsMessages.CannotReferSelf);

        var alreadyReferred = await referralRepository.AnyAsync(r => r.ReferredUserId == request.ReferredUserId, cancellationToken: cancellationToken);
        if (alreadyReferred)
            return Result.BadRequest<Guid>(ReferralsMessages.AlreadyReferred);

        var referral = new Referral
        {
            Id = Guid.NewGuid(),
            ReferrerUserId = referralCode.UserId,
            ReferredUserId = request.ReferredUserId,
            Code = request.Code,
            Status = ReferralStatuses.Pending,
        };

        await referralRepository.AddAsync(referral);

        return Result.Created(referral.Id, "Referral code redeemed successfully.");
    }
}
