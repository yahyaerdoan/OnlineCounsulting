using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.CreateFooterInfo;

public class CreateFooterInfoValidator : AbstractValidator<CreateFooterInfoCommand>
{
    public CreateFooterInfoValidator()
    {
        _ = RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
        _ = RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
    }
}
