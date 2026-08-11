using FluentValidation;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.BookDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Books;

internal class CreateBookValidator : AbstractValidator<CreateBookDto>
{
    public CreateBookValidator()
    {
        RuleFor(x => x.Notes).NotEmpty().WithMessage("Title cannot be empty.");
    }
}
