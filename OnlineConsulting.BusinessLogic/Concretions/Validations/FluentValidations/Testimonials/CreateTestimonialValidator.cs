using FluentValidation;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.BusinessLogic.Concretions.Validations.ValidationMessages;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.TestimonialDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Testimonials;

internal class CreateTestimonialValidator : AbstractValidator<CreateTestimonialDto>
{
    public CreateTestimonialValidator()
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

        RuleFor(x => x.Image)
        .Must(image => image is null || IsValidImage(image))
        .WithMessage(ValidationMessage.TheImageMustFormat);


    }
    private static bool IsValidImage(IFormFile image)
    {
        if (image is null)
            return false;

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

        return allowedExtensions.Contains(extension);
    }
}
