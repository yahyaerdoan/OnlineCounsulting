using FluentValidation;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Orders;

public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidator()
    {

    }
}
