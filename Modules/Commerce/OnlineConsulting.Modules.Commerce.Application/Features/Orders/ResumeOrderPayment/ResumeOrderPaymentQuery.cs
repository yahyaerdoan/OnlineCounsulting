using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using SharedPaymentStatuses = OnlineConsulting.SharedKernel.Payments.PaymentStatuses;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.ResumeOrderPayment;

/// <summary>Returns the existing payment's client secret for an unpaid order; null when it is already paid or still processing.</summary>
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
