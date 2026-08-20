using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.UpdateServiceArea;

public class UpdateServiceAreaValidator : AbstractValidator<UpdateServiceAreaCommand>
{
    public UpdateServiceAreaValidator()
    {
        _ = RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.State).NotEmpty().MaximumLength(50);
        _ = RuleFor(x => x.IntroText).MaximumLength(2000);
    }
}
