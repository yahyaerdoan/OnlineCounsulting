using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.UpdatePageBanner;

public class UpdatePageBannerValidator : AbstractValidator<UpdatePageBannerCommand>
{
    public UpdatePageBannerValidator()
    {
        _ = RuleFor(x => x.Id).NotEmpty();
        _ = RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        _ = RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
    }
}
