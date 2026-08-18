using FluentValidation;

namespace OnlineConsulting.Modules.Referrals.Application.Features.Referrals.CompleteReferral;

public class CompleteReferralValidator : AbstractValidator<CompleteReferralCommand>
{
    public CompleteReferralValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.RewardAmount).GreaterThan(0);
    }
}
