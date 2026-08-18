using FluentValidation;

namespace OnlineConsulting.Modules.Referrals.Application.Features.Referrals.RedeemReferralCode;

public class RedeemReferralCodeValidator : AbstractValidator<RedeemReferralCodeCommand>
{
    public RedeemReferralCodeValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
    }
}
