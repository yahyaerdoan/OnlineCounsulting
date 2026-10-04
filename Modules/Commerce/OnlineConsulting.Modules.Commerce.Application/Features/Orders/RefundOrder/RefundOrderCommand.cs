using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.RefundOrder;

/// <summary>Refunds a paid order through the gateway that took the payment; a null <c>Amount</c> refunds in full.</summary>
public record RefundOrderCommand(Guid OrderId, decimal? Amount = null) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [CommerceOperationClaims.Admin, CommerceOperationClaims.Write, GlobalOperationClaims.SuperAdmin];
}

public class RefundOrderHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, IOrderNotifier notifier) : IRequestHandler<RefundOrderCommand, OperationResult>
{
    public async Task<OperationResult> Handle(RefundOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(o => o.Id == request.OrderId, cancellationToken: cancellationToken);

        if (order is null)
        {
            return Result.NotFound($"Order {request.OrderId} was not found.");
        }

        if (!order.CanBeRefunded)
        {
            return Result.Conflict($"Order {request.OrderId} cannot be refunded from payment status '{order.PaymentStatus}' - only a paid order can be refunded.");
        }

        if (order.PaymentProvider is null || order.ProviderPaymentId is null)
        {
            return Result.Conflict($"Order {request.OrderId} has no recorded payment to refund.");
        }

        var gateway = serviceProvider.GetKeyedService<IPaymentGateway>(order.PaymentProvider);

        if (gateway is null)
        {
            return Result.InternalServerError($"Unknown payment provider '{order.PaymentProvider}' - cannot route the refund.");
        }

        var failure = await PaymentGatewayCall.RunAsync(() =>
        gateway.RefundAsync(order.ProviderPaymentId, request.Amount, cancellationToken), $"Refund failed for order {request.OrderId}. Please try again or contact support.");

        if (failure is not null)
        {
            return failure;
        }

        order.Refund();

        _ = await orderRepository.UpdateAsync(order, cancellationToken: cancellationToken);

        await notifier.RefundedAsync(order, request.Amount, cancellationToken);

        return Result.Success("Order refunded successfully.");
    }
}
