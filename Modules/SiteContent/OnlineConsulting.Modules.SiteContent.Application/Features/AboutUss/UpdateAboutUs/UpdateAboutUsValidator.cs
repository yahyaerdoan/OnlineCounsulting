using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.UpdateAboutUs;

public class UpdateAboutUsValidator : AbstractValidator<UpdateAboutUsCommand>
{
    public UpdateAboutUsValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
    }
}
