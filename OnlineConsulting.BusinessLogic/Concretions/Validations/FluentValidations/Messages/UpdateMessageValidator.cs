using FluentValidation;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.MessageDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Messages;

public class UpdateMessageValidator : AbstractValidator<UpdateMessageDto>
{
    public UpdateMessageValidator()
    {

    }
}
