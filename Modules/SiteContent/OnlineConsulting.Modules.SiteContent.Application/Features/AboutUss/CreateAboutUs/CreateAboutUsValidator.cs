using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.CreateAboutUs;

public class CreateAboutUsValidator : AbstractValidator<CreateAboutUsCommand>
{
    public CreateAboutUsValidator()
    {
        _ = RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
    }
}
