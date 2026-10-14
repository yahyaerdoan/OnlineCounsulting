using FluentValidation;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Domain.Landing;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.Abstractions;

public class LandingBlockFieldsValidator : AbstractValidator<ILandingBlockFields>
{
    public LandingBlockFieldsValidator()
    {
        _ = RuleFor(x => x.Section).NotEmpty().Must(LandingSections.All.Contains)
            .WithMessage(string.Format(SiteContentMessages.UnknownLandingSectionFormat, string.Join(", ", LandingSections.All)));
        _ = RuleFor(x => x.Title).NotEmpty().MaximumLength(LandingLimits.Title);
        _ = RuleFor(x => x.Body).NotEmpty().MaximumLength(LandingLimits.Body);
        _ = RuleFor(x => x.Icon).MaximumLength(LandingLimits.Icon);
    }
}
