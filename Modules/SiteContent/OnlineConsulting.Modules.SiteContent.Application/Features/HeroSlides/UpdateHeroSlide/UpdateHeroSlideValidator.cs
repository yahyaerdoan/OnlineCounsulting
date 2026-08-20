using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.UpdateHeroSlide;

public class UpdateHeroSlideValidator : AbstractValidator<UpdateHeroSlideCommand>
{
    public UpdateHeroSlideValidator()
    {
        _ = RuleFor(x => x.Id).NotEmpty();
        _ = RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        _ = RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
    }
}
