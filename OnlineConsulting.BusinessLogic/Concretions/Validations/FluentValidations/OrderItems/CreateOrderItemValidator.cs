using FluentValidation;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderItemDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.OrderItems;

public class CreateOrderItemValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemValidator()
    {

    }
}
