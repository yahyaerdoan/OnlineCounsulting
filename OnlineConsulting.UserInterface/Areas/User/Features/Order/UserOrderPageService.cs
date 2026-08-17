using OnlineConsulting.UserInterface.Areas.User.Features.UserAddress;
using OnlineConsulting.UserInterface.Features.Category;
using OnlineConsulting.UserInterface.Features.Service;
using OnlineConsulting.UserInterface.Infrastructure.Media;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Order;

public class UserOrderPageService(
    IOrderService orderService,
    IUserAddressService userAddressService,
    IServiceCatalogService serviceCatalogService,
    ICategoryService categoryService,
    IMediaService mediaService) : IUserOrderPageService
{
    public async Task<List<UserOrderListItemViewModel>> GetMyOrdersAsync(CancellationToken cancellationToken = default)
    {
        var orders = await orderService.GetAllAsync(cancellationToken);
        return orders
            .OrderByDescending(o => o.CreatedDate)
            .Select(ToListItem)
            .ToList();
    }

    public async Task<UserOrderDetailViewModel> GetMyOrderDetailAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var detail = await orderService.GetDetailAsync(orderId, cancellationToken);
        if (detail is null)
            return new UserOrderDetailViewModel();

        var addresses = await userAddressService.GetAllAsync(cancellationToken);

        var model = new UserOrderDetailViewModel
        {
            Order = ToListItem(detail.Order),
            ShippingAddress = ToAddress(addresses.FirstOrDefault(a => a.Id == detail.ShippingAddressId)),
            InvoiceAddress = ToAddress(addresses.FirstOrDefault(a => a.Id == detail.InvoiceAddressId)),
        };

        foreach (var item in detail.Items)
        {
            var service = await serviceCatalogService.GetByIdAsync(item.ServiceId, cancellationToken);
            var category = service is null ? null : await categoryService.GetByIdAsync(service.CategoryId, cancellationToken);

            model.OrderItems.Add(new UserOrderItemViewModel(
                item.Id,
                item.ServiceId,
                service?.Title ?? "Service",
                await mediaService.ResolveUrlAsync(service?.CoverMediaAssetId, cancellationToken),
                category?.Title ?? string.Empty,
                item.Quantity,
                item.UnitPrice,
                item.TaxRate,
                item.TaxAmount,
                item.SubTotalPrice,
                item.TotalPrice));
        }

        return model;
    }

    public async Task<UserDashboardStatsViewModel> GetMyStatsAsync(CancellationToken cancellationToken = default)
    {
        // GetOrderStats covers totals; the per-status breakdown the old screen showed is counted off the same
        // order list rather than four extra Api round-trips.
        var stats = await orderService.GetStatsAsync(cancellationToken);
        var orders = await orderService.GetAllAsync(cancellationToken);

        return new UserDashboardStatsViewModel(
            stats?.TotalOrders ?? orders.Count,
            orders.Count(o => o.OrderStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase)),
            orders.Count(o => o.PaymentStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase)),
            orders.Count(o => o.OrderStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase)),
            stats?.TotalSpent ?? 0);
    }

    private static UserOrderListItemViewModel ToListItem(OrderResponse order) =>
        new(order.Id, order.OrderNumber, order.OrderStatus, order.PaymentStatus, order.TotalPrice, order.CreatedDate);

    private static UserOrderAddressViewModel ToAddress(UserAddressResponse? address) =>
        address is null
            ? new UserOrderAddressViewModel(string.Empty, null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
            : new UserOrderAddressViewModel(address.AddressName, address.CompanyName, address.Country, address.AddressLine, address.City, address.State, address.Zipcode);
}
