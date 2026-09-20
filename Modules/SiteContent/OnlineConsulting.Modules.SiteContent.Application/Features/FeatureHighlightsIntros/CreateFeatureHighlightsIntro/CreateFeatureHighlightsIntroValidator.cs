using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.CreateFeatureHighlightsIntro;

public class CreateFeatureHighlightsIntroValidator : AbstractValidator<CreateFeatureHighlightsIntroCommand>
{
    public CreateFeatureHighlightsIntroValidator()
    {
        _ = RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}
