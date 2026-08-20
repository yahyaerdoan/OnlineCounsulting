using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.UpdateFooterInfo;

public class UpdateFooterInfoValidator : AbstractValidator<UpdateFooterInfoCommand>
{
    public UpdateFooterInfoValidator()
    {
        _ = RuleFor(x => x.Id).NotEmpty();
        _ = RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
        _ = RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
    }
}
