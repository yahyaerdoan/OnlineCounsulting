using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.CreateTestimonial;

public class CreateTestimonialValidator : AbstractValidator<CreateTestimonialCommand>
{
    public CreateTestimonialValidator()
    {
        _ = RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        _ = RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
    }
}
