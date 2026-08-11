using FluentValidation;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.BusinessLogic.Concretions.Validations.ValidationMessages;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryItemDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.GalleryItems;

internal class CreateGalleryItemValidator : AbstractValidator<CreateGalleryItemDto>
{
    public CreateGalleryItemValidator()
    {
        RuleFor(x => x.Image)
            .NotEmpty().WithMessage(ValidationMessage.TheImageNotEmpty)
            .Must(IsValidImage).WithMessage(ValidationMessage.TheImageMustFormat);

        RuleFor(x => x.Description)
          .NotEmpty().WithMessage(ValidationMessage.TheDescriptionNotEmpty)
          .MinimumLength(5).WithMessage(ValidationMessage.TheDescriptionMinimumLength);

        RuleFor(x => x.GalleryCategoryIds)
            .NotNull().WithMessage("Gallery categories must be provided!")
            .Must(ids => ids.Count != 0).WithMessage("At least one category must be selected!");
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
