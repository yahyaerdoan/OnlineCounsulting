using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.UpdateFeatureHighlight;

public class UpdateFeatureHighlightValidator : AbstractValidator<UpdateFeatureHighlightCommand>
{
    public UpdateFeatureHighlightValidator()
    {
        _ = RuleFor(x => x.Id).NotEmpty();
        _ = RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        _ = RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
    }
}
