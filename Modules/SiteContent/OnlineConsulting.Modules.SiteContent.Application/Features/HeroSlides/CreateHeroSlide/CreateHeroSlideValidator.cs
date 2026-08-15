using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.CreateHeroSlide;

public class CreateHeroSlideValidator : AbstractValidator<CreateHeroSlideCommand>
{
    public CreateHeroSlideValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
    }
}
