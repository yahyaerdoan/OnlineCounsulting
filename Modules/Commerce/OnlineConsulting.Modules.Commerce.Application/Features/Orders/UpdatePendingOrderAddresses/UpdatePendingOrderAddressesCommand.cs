using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using OrderPaymentStatuses = OnlineConsulting.Modules.Commerce.Application.Features.Orders.Constants.PaymentStatuses;
using OrderStatuses = OnlineConsulting.Modules.Commerce.Application.Features.Orders.Constants.OrderStatuses;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.UpdatePendingOrderAddresses;

/// <summary>Points the caller's own still-unpaid order at their current default shipping and billing addresses, so going back from the
/// payment step to change an address keeps the same order and payment instead of opening a second one. Amounts don't depend on the
/// address and the receipt is only issued once paid (from the order's addresses at that time), so nothing else needs updating.</summary>
public record UpdatePendingOrderAddressesCommand(Guid OrderId, Guid UserId) : IRequest<OperationResult>, ITransactionAddRequest;

public class UpdatePendingOrderAddressesHandler(IOrderRepository orderRepository, IUserAddressRepository userAddressRepository)
    : IRequestHandler<UpdatePendingOrderAddressesCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdatePendingOrderAddressesCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, cancellationToken: cancellationToken);
        if (order is null)
        {
            return Result.NotFound($"Order {request.OrderId} was not found.");
        }

        if (order.PaymentStatus != OrderPaymentStatuses.Pending || order.OrderStatus == OrderStatuses.Cancelled)
        {
            return Result.Conflict("Only a pending, unpaid order's addresses can be changed.");
        }

        var shippingAddress = await userAddressRepository.GetAsync(a => a.UserId == request.UserId && a.IsShippingAddress, enableTracking: false, cancellationToken: cancellationToken);
        if (shippingAddress is null)
        {
            return Result.Conflict(AddressMessages.ShippingAddressNotFound);
        }

        var billingAddress = await userAddressRepository.GetAsync(a => a.UserId == request.UserId && a.IsBillingAddress, enableTracking: false, cancellationToken: cancellationToken);
        if (billingAddress is null)
        {
            return Result.Conflict(AddressMessages.BillingAddressNotFound);
        }

        order.ShippingAddressId = shippingAddress.Id;
        order.InvoiceAddressId = billingAddress.Id;
        _ = await orderRepository.UpdateAsync(order);

        return Result.Success("Order addresses updated.");
    }
}
