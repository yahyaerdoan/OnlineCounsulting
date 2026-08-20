using FluentValidation;

namespace OnlineConsulting.Modules.Referrals.Application.Features.Referrals.RedeemReferralCode;

public class RedeemReferralCodeValidator : AbstractValidator<RedeemReferralCodeCommand>
{
    public RedeemReferralCodeValidator()
    {
        _ = RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
    }
}
