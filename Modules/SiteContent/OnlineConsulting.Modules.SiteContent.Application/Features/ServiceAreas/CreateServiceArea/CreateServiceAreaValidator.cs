using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.CreateServiceArea;

public class CreateServiceAreaValidator : AbstractValidator<CreateServiceAreaCommand>
{
    public CreateServiceAreaValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.State).NotEmpty().MaximumLength(50);
        RuleFor(x => x.IntroText).MaximumLength(2000);
    }
}
