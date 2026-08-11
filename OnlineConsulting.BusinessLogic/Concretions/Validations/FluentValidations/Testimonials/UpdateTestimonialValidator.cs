using FluentValidation;
using OnlineConsulting.BusinessLogic.Concretions.Validations.ValidationMessages;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.TestimonialDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Testimonials;

internal class UpdateTestimonialValidator : AbstractValidator<UpdateTestimonialDto>
{
    public UpdateTestimonialValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ValidationMessage.TheTitleNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheTitleMinimumLength);

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name cannot be empty!")
            .MinimumLength(5).WithMessage("First name must be over 5 characters!");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name cannot be empty!")
            .MinimumLength(5).WithMessage("Last name must be over 5 characters!");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ValidationMessage.TheDescriptionNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheDescriptionMinimumLength);
    }
}
