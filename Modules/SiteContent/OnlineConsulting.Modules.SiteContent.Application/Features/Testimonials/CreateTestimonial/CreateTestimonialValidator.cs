using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.CreateTestimonial;

public class CreateTestimonialValidator : AbstractValidator<CreateTestimonialCommand>
{
    public CreateTestimonialValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
    }
}
