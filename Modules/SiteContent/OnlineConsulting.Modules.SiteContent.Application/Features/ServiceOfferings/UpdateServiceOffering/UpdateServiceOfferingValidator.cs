using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.UpdateServiceOffering;

public class UpdateServiceOfferingValidator : AbstractValidator<UpdateServiceOfferingCommand>
{
    public UpdateServiceOfferingValidator()
    {
        _ = RuleFor(x => x.Title).NotEmpty().MinimumLength(5).MaximumLength(200);
        _ = RuleFor(x => x.Description).NotEmpty().MinimumLength(5).MaximumLength(2000);
        _ = RuleFor(x => x.Icon).NotEmpty().MaximumLength(2000);
        _ = RuleFor(x => x.IconColor).MaximumLength(7);
    }
}
