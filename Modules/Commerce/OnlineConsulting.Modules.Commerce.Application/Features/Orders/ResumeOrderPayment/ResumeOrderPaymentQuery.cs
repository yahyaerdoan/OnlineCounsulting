using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using OrderPaymentStatuses = OnlineConsulting.Modules.Commerce.Application.Features.Orders.Constants.PaymentStatuses;
using SharedPaymentStatuses = OnlineConsulting.SharedKernel.Payments.PaymentStatuses;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.ResumeOrderPayment;

/// <summary>
/// Re-fetches a PaymentClientSecret for an unpaid order, e.g. after a page reload - never creates a new charge.
/// A null <c>PaymentClientSecret</c> in the result signals "already paid" (or still processing); a cancelled order fails outright
/// since checkout never reserved a basket for it to resume. When the provider already reports the charge as succeeded but the
/// webhook hasn't landed yet, the order is settled here, so the customer is never offered a second payment for it.
/// </summary>
public record ResumeOrderPaymentQuery(Guid OrderId, Guid UserId) : IRequest<OperationDataResult<CreateOrderResult>>;

public class ResumeOrderPaymentHandler(IOrderRepository orderRepository, IPaymentGateway paymentGateway, IPublisher publisher) : IRequestHandler<ResumeOrderPaymentQuery, OperationDataResult<CreateOrderResult>>
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
            return Result.Success(new CreateOrderResult(order.Id, PaymentClientSecret: null, order.OrderNumber), "This order has already been paid.");
        }

        if (order.PaymentStatus == OrderPaymentStatuses.Cancelled)
        {
            return Result.Conflict<CreateOrderResult>($"Order {order.OrderNumber} was cancelled and can no longer be paid.");
        }

        if (order.ProviderPaymentId is null)
        {
            return Result.Conflict<CreateOrderResult>("This order has no payment to resume.");
        }

        var (failure, status) = await PaymentGatewayCall.RunWithResultAsync(() =>
        paymentGateway.GetStatusAsync(order.ProviderPaymentId, cancellationToken), $"Could not retrieve payment status for order {order.OrderNumber}. Please try again.");

        if (failure is not null || status is null)
        {
            return Result.BadGateway<CreateOrderResult>(failure?.Detail ?? "Could not retrieve payment status.");
        }

        if (status.Status == SharedPaymentStatuses.Succeeded)
        {
            await publisher.Publish(new PaymentSucceededNotification(order.Id.ToString(), order.ProviderPaymentId, paymentGateway.ProviderName), cancellationToken);
            return Result.Success(new CreateOrderResult(order.Id, PaymentClientSecret: null, order.OrderNumber), "This order has already been paid.");
        }

        return status.Status == SharedPaymentStatuses.Processing
            ? Result.Success(new CreateOrderResult(order.Id, PaymentClientSecret: null, order.OrderNumber), "Your payment is processing.")
            : Result.Success(new CreateOrderResult(order.Id, status.ClientSecret, order.OrderNumber), "Payment resumed.");
    }
}
