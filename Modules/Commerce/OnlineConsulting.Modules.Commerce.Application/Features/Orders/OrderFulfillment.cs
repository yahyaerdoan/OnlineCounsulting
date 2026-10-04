using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders;

/// <summary>The basket is left alone at checkout for an asynchronous payment, so completing the paid order is what finally clears it.</summary>
public class OrderFulfillment(IBasketRepository basketRepository, IOrderItemRepository orderItemRepository,
    IInvoiceService invoiceService, IOrderNotifier notifier) : IOrderFulfillment
{
    public async Task CompletePaidAsync(Order order, CancellationToken cancellationToken = default)
    {
        await ClearBasketAsync(order.UserId, cancellationToken);

        var items = await orderItemRepository.GetAllAsync(i => i.OrderId == order.Id, cancellationToken: cancellationToken);
        var invoice = await invoiceService.IssueForPaidOrderAsync(order, [.. items], cancellationToken);

        await notifier.PaidAsync(order, items.Count, items.Sum(i => i.TotalPrice), invoice.Id, cancellationToken);
    }

    private async Task ClearBasketAsync(Guid userId, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetForOwnerAsync(userId, null, cancellationToken: cancellationToken);
        if (basket is null)
        {
            return;
        }

        basket.Clear();
        _ = await basketRepository.DeleteAsync(basket, cancellationToken: cancellationToken);
    }
}
