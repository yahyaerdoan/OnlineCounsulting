using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using OrderPaymentStatuses = OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts.PaymentStatuses;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.ResumeOrderPayment;

/// <summary>Re-fetches a PaymentClientSecret for an unpaid order, e.g. after a page reload - never creates a new charge.</summary>
public record ResumeOrderPaymentQuery(Guid OrderId, Guid UserId) : IRequest<OperationDataResult<CreateOrderResult>>;

public class ResumeOrderPaymentHandler(IOrderRepository orderRepository, IPaymentGateway paymentGateway) : IRequestHandler<ResumeOrderPaymentQuery, OperationDataResult<CreateOrderResult>>
{
    public async Task<OperationDataResult<CreateOrderResult>> Handle(ResumeOrderPaymentQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, enableTracking: false, cancellationToken: cancellationToken);
        if (order is null)
        {
            return Result.NotFound<CreateOrderResult>($"Order {request.OrderId} was not found.");
        }

        if (order.PaymentStatus == OrderPaymentStatuses.Paid)
        {
            // No client secret signals "already paid" - Checkout.razor shows the Confirmed screen for this.
            return Result.Success(new CreateOrderResult(order.Id, PaymentClientSecret: null, order.OrderNumber), "This order has already been paid.");
        }

        if (order.PaymentStatus == OrderPaymentStatuses.Cancelled)
        {
            // Expired/failed - the basket was never touched by checkout, so there's nothing to resume;
            // Checkout.razor falls back to the normal basket flow when this fails.
            return Result.BadRequest<CreateOrderResult>($"Order {order.OrderNumber} was cancelled and can no longer be paid.");
        }

        if (order.ProviderPaymentId is null)
        {
            return Result.BadRequest<CreateOrderResult>("This order has no payment to resume.");
        }

        var (failure, status) = await PaymentGatewayCall.RunWithResultAsync(() => paymentGateway.GetStatusAsync(order.ProviderPaymentId, cancellationToken),
            $"Could not retrieve payment status for order {order.OrderNumber}. Please try again.");
        if (failure is not null || status is null)
        {
            return Result.BadRequest<CreateOrderResult>(failure?.Detail ?? "Could not retrieve payment status.");
        }

        return Result.Success(new CreateOrderResult(order.Id, status.ClientSecret, order.OrderNumber), "Payment resumed.");
    }
}
