using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Persistence;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders;

/// <summary>The basket is left alone at checkout for an asynchronous payment, so completing the paid order is what finally clears it.</summary>
public class OrderFulfillment(IBasketRepository basketRepository, IBasketItemRepository basketItemRepository, IOrderItemRepository orderItemRepository,
    IInvoiceService invoiceService, IOrderNotifier notifier) : IOrderFulfillment
{
    public async Task CompletePaidAsync(Order order, CancellationToken cancellationToken = default)
    {
        await ClearBasketAsync(order.UserId, cancellationToken);

        var items = await orderItemRepository.GetListAsync(i => i.OrderId == order.Id, orderBy: q => q.OrderBy(i => i.Id), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var invoice = await invoiceService.IssueForPaidOrderAsync(order, [.. items.Items], cancellationToken);

        await notifier.PaidAsync(order, items.Items.Count, items.Items.Sum(i => i.TotalPrice), invoice.Id, cancellationToken);
    }

    private async Task ClearBasketAsync(Guid userId, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetAsync(b => b.UserId == userId, cancellationToken: cancellationToken);
        if (basket is null)
        {
            return;
        }

        var basketItems = await basketItemRepository.GetListAsync(i => i.BasketId == basket.Id, orderBy: q => q.OrderBy(i => i.Id), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        foreach (var basketItem in basketItems.Items)
        {
            _ = await basketItemRepository.DeleteAsync(basketItem);
        }

        _ = await basketRepository.DeleteAsync(basket);
    }
}
