using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.CreateServiceOffering;

public class CreateServiceOfferingValidator : AbstractValidator<CreateServiceOfferingCommand>
{
    public CreateServiceOfferingValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(5).MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(5).MaximumLength(2000);
        RuleFor(x => x.Icon).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.IconColor).MaximumLength(7);
    }
}
