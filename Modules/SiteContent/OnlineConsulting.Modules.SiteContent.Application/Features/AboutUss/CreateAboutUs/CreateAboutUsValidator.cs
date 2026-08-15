using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.CreateAboutUs;

public class CreateAboutUsValidator : AbstractValidator<CreateAboutUsCommand>
{
    public CreateAboutUsValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
    }
}
