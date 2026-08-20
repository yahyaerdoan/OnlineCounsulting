using FluentValidation;

namespace OnlineConsulting.Modules.Referrals.Application.Features.Referrals.CompleteReferral;

public class CompleteReferralValidator : AbstractValidator<CompleteReferralCommand>
{
    public CompleteReferralValidator()
    {
        _ = RuleFor(x => x.Id).NotEmpty();
        _ = RuleFor(x => x.RewardAmount).GreaterThan(0);
    }
}
