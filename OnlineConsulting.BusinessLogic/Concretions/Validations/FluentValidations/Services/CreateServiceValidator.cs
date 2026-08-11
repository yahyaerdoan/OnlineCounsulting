using FluentValidation;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.BusinessLogic.Concretions.Validations.ValidationMessages;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Services;

public class CreateServiceValidator : AbstractValidator<CreateServiceDto>
{
    public CreateServiceValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category cannot be empty.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ValidationMessage.TheTitleNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheTitleMinimumLength);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.")
            .NotEmpty().WithMessage("Price cannot be empty.");

        RuleFor(x => x.DiscountedPrice)
            .GreaterThan(0).WithMessage("Discounted Price must be greater than 0.")
            .NotEmpty().WithMessage("Discounted Price cannot be empty.");

        RuleFor(x => x.DiscountRate)
            .InclusiveBetween(-1, 100).WithMessage("Discount Rate must be between 0 and 100.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ValidationMessage.TheDescriptionNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheDescriptionMinimumLength);

        RuleFor(x => x.DetailedDescription)
            .NotEmpty().WithMessage(ValidationMessage.TheDescriptionNotEmpty)
            .MinimumLength(25).WithMessage(ValidationMessage.TheDescriptionMinimumLength);

        RuleFor(x => x.Images)
            .NotEmpty().WithMessage(ValidationMessage.TheImageNotEmpty)
            .ForEach(x => x.Must(IsValidImage)
            .WithMessage(ValidationMessage.TheImageMustFormat));
    }
    private bool IsValidImage(IFormFile? image)
    {
        if (image is null)
            return false;

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

        return allowedExtensions.Contains(extension);
    }
}
