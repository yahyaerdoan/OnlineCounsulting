using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.UpdateAboutUs;

public class UpdateAboutUsValidator : AbstractValidator<UpdateAboutUsCommand>
{
    public UpdateAboutUsValidator()
    {
        _ = RuleFor(x => x.Id).NotEmpty();
        _ = RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
    }
}
