using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.CreateServiceOffering;

public class CreateServiceOfferingValidator : AbstractValidator<CreateServiceOfferingCommand>
{
    public CreateServiceOfferingValidator()
    {
        _ = RuleFor(x => x.Title).NotEmpty().MinimumLength(5).MaximumLength(200);
        _ = RuleFor(x => x.Description).NotEmpty().MinimumLength(5).MaximumLength(2000);
        _ = RuleFor(x => x.Icon).NotEmpty().MaximumLength(2000);
        _ = RuleFor(x => x.IconColor).MaximumLength(7);
    }
}
