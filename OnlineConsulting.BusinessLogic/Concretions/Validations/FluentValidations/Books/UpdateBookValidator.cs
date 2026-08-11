using FluentValidation;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.BookDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Books;

internal class UpdateBookValidator : AbstractValidator<UpdateBookDto>
{
    public UpdateBookValidator()
    {

    }
}
