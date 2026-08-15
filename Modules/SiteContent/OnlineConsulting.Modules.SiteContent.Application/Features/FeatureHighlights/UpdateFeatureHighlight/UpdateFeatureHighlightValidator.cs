using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.UpdateFeatureHighlight;

public class UpdateFeatureHighlightValidator : AbstractValidator<UpdateFeatureHighlightCommand>
{
    public UpdateFeatureHighlightValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
    }
}
