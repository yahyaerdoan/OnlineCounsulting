using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.CancelPendingOrder;

/// <summary>Cancels the caller's unpaid order; the basket is left as is.</summary>
public record CancelPendingOrderCommand(Guid OrderId, Guid UserId) : IRequest<OperationResult>, ICommerceTransactionRequest;

public class CancelPendingOrderHandler(IOrderRepository orderRepository) : IRequestHandler<CancelPendingOrderCommand, OperationResult>
{
    public async Task<OperationResult> Handle(CancelPendingOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, cancellationToken: cancellationToken);
        if (order is null)
        {
            return Result.NotFound($"Order {request.OrderId} was not found.");
        }

        if (!order.IsAwaitingPayment)
        {
            return Result.Conflict("Only a pending, unpaid order can be cancelled this way.");
        }

        order.Cancel();

        _ = await orderRepository.UpdateAsync(order, cancellationToken: cancellationToken);

        return Result.Success("Order cancelled - your cart is unchanged.");
    }
}
