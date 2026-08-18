using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.UpdateServiceArea;

public class UpdateServiceAreaValidator : AbstractValidator<UpdateServiceAreaCommand>
{
    public UpdateServiceAreaValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.State).NotEmpty().MaximumLength(50);
        RuleFor(x => x.IntroText).MaximumLength(2000);
    }
}
