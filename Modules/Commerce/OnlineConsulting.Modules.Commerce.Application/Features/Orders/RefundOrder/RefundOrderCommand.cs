using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;
using OrderPaymentStatuses = OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts.PaymentStatuses;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.RefundOrder;

/// <summary>Amount null means a full refund (IPaymentGateway.RefundAsync convention). Resolves the gateway keyed by Order.PaymentProvider - not the app's currently-active provider, which may differ from whichever one actually processed this order (same routing precedent as the webhook endpoint).</summary>
public record RefundOrderCommand(Guid OrderId, decimal? Amount = null) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];
}

public class RefundOrderHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider) : IRequestHandler<RefundOrderCommand, OperationResult>
{
    public async Task<OperationResult> Handle(RefundOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(o => o.Id == request.OrderId, cancellationToken: cancellationToken);
        if (order is null)
            return Result.NotFound($"Order {request.OrderId} was not found.");

        if (order.PaymentStatus != OrderPaymentStatuses.Paid)
            return Result.BadRequest($"Order {request.OrderId} cannot be refunded from payment status '{order.PaymentStatus}' - only a paid order can be refunded.");

        if (order.PaymentProvider is null || order.ProviderPaymentId is null)
            return Result.BadRequest($"Order {request.OrderId} has no recorded payment to refund.");

        var gateway = serviceProvider.GetKeyedService<IPaymentGateway>(order.PaymentProvider);
        if (gateway is null)
            return Result.BadRequest($"Unknown payment provider '{order.PaymentProvider}' - cannot route the refund.");

        await gateway.RefundAsync(order.ProviderPaymentId, request.Amount, cancellationToken);

        order.PaymentStatus = OrderPaymentStatuses.Refunded;
        await orderRepository.UpdateAsync(order);

        return Result.Success("Order refunded successfully.");
    }
}
