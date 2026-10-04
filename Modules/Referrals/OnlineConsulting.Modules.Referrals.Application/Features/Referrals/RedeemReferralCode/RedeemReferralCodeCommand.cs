using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Referrals.Application.Common;
using OnlineConsulting.Modules.Referrals.Application.Features.ReferralCodes.Abstractions;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Abstractions;
using OnlineConsulting.Modules.Referrals.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Referrals.Application.Features.Referrals.RedeemReferralCode;

/// <summary>A user can redeem at most one referral code, ever - enforced here (not just at signup) regardless of when they first hear about the program.</summary>
public record RedeemReferralCodeCommand(Guid ReferredUserId, string Code) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    public string[] Roles => [];
}

public class RedeemReferralCodeHandler(IReferralRepository referralRepository, IReferralCodeRepository referralCodeRepository) : IRequestHandler<RedeemReferralCodeCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(RedeemReferralCodeCommand request, CancellationToken cancellationToken)
    {
        var referralCode = await referralCodeRepository.GetAsync(c => c.Code == request.Code, cancellationToken: cancellationToken);

        if (referralCode is null)
        {
            return Result.NotFound<Guid>(ReferralsMessages.InvalidCode);
        }

        if (referralCode.UserId == request.ReferredUserId)
        {
            return Result.UnprocessableContent<Guid>(ReferralsMessages.CannotReferSelf);
        }

        var alreadyReferred = await referralRepository.AnyAsync(r => r.ReferredUserId == request.ReferredUserId, cancellationToken: cancellationToken);

        if (alreadyReferred)
        {
            return Result.Conflict<Guid>(ReferralsMessages.AlreadyReferred);
        }

        var referral = Referral.Create(referralCode.UserId, request.ReferredUserId, request.Code);

        _ = await referralRepository.AddAsync(referral, cancellationToken: cancellationToken);

        return Result.Created(referral.Id, "Referral code redeemed successfully.");
    }
}
