using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.UpdateTestimonial;

public class UpdateTestimonialValidator : AbstractValidator<UpdateTestimonialCommand>
{
    public UpdateTestimonialValidator()
    {
        _ = RuleFor(x => x.Id).NotEmpty();
        _ = RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        _ = RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
    }
}
