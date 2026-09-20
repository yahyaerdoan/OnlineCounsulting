using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.UpdateFeatureHighlightsIntro;

public class UpdateFeatureHighlightsIntroValidator : AbstractValidator<UpdateFeatureHighlightsIntroCommand>
{
    public UpdateFeatureHighlightsIntroValidator()
    {
        _ = RuleFor(x => x.Id).NotEmpty();
        _ = RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}
