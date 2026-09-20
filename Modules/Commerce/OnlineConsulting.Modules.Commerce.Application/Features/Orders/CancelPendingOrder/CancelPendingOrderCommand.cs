using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using OrderPaymentStatuses = OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts.PaymentStatuses;
using OrderStatuses = OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts.OrderStatuses;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.CancelPendingOrder;

/// <summary>Cancels a pending order. The basket was never touched by checkout (CreateOrderFromBasketHandler
/// only clears it once payment is actually confirmed), so there's nothing left to restore here.</summary>
public record CancelPendingOrderCommand(Guid OrderId, Guid UserId) : IRequest<OperationResult>, ITransactionAddRequest;

public class CancelPendingOrderHandler(IOrderRepository orderRepository)
    : IRequestHandler<CancelPendingOrderCommand, OperationResult>
{
    public async Task<OperationResult> Handle(CancelPendingOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, cancellationToken: cancellationToken);
        if (order is null)
        {
            return Result.NotFound($"Order {request.OrderId} was not found.");
        }

        if (order.PaymentStatus != OrderPaymentStatuses.Pending)
        {
            return Result.BadRequest("Only a pending, unpaid order can be cancelled this way.");
        }

        order.PaymentStatus = OrderPaymentStatuses.Cancelled;
        order.OrderStatus = OrderStatuses.Cancelled;
        _ = await orderRepository.UpdateAsync(order);

        return Result.Success("Order cancelled - your cart is unchanged.");
    }
}
