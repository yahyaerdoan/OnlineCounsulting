using FluentValidation;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderItemDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.OrderItems;

public class UpdateOrderItemValidator : AbstractValidator<UpdateOrderItemDto>
{
    public UpdateOrderItemValidator()
    {

    }
}
