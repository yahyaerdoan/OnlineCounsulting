using FluentValidation;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Domain.Landing;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.ReorderLandingBlocks;

public class ReorderLandingBlocksValidator : AbstractValidator<ReorderLandingBlocksCommand>
{
    public ReorderLandingBlocksValidator()
    {
        _ = RuleFor(x => x.Section).NotEmpty().Must(LandingSections.All.Contains)
            .WithMessage(string.Format(SiteContentMessages.UnknownLandingSectionFormat, string.Join(", ", LandingSections.All)));
        _ = RuleFor(x => x.OrderedIds).NotEmpty()
            .Must(ids => ids.Distinct().Count() == ids.Count).WithMessage(SiteContentMessages.LandingBlockOrderDuplicate);
    }
}
