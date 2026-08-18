using FluentValidation;

namespace OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.SpendAccountCredit;

public class SpendAccountCreditValidator : AbstractValidator<SpendAccountCreditCommand>
{
    public SpendAccountCreditValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty();
        RuleFor(x => x.SourceType).NotEmpty();
        RuleFor(x => x.SourceId).NotEmpty();
    }
}
