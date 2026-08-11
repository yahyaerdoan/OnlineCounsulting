using FluentValidation;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.NewsletterDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Newsletters;

internal class UpdateNewsletterValidator : AbstractValidator<UpdateNewsletterDto>
{
    public UpdateNewsletterValidator()
    {

    }
}
