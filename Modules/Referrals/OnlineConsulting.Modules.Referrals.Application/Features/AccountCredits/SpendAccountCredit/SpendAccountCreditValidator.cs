using FluentValidation;

namespace OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.SpendAccountCredit;

public class SpendAccountCreditValidator : AbstractValidator<SpendAccountCreditCommand>
{
    public SpendAccountCreditValidator()
    {
        _ = RuleFor(x => x.UserId).NotEmpty();
        _ = RuleFor(x => x.Amount).GreaterThan(0);
        _ = RuleFor(x => x.Reason).NotEmpty();
        _ = RuleFor(x => x.SourceType).NotEmpty();
        _ = RuleFor(x => x.SourceId).NotEmpty();
    }
}
