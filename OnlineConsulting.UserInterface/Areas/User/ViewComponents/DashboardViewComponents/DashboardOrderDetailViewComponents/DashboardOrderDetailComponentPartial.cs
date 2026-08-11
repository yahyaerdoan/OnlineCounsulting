using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderDtos;
using System.Diagnostics;

namespace OnlineConsulting.UserInterface.Areas.User.ViewComponents.DashboardViewComponents.DashboardOrderDetailViewComponents;

public class DashboardOrderDetailComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    #region GetOrderDetailByIdAsync Step-by-Step Explanation
    //public async Task<IViewComponentResult> InvokeAsync(Guid orderId)
    //{
    //    // Get current user
    //    var userResult = await serviceManager.SystemUserService.GetCurrentUserAsync();
    //    if (!userResult.IsSuccessful)
    //        return EmptyViewModel();

    //    // Get user's orders
    //    var ordersResult = await serviceManager.OrderService.GetOrdersByUserIdAsync(userResult.Data.Id);
    //    if (!ordersResult.IsSuccessful || ordersResult.Data is null)
    //        return EmptyViewModel();

    //    // Find specific order
    //    var order = ordersResult.Data.FirstOrDefault(o => o.Id == orderId);
    //    if (order is null)
    //        return EmptyViewModel();

    //    // Get user's addresses
    //    var shippingAddress = await serviceManager.UserAddressService.GetByIdAsync<ResultUserAddressDto>(order.ShippingAddressId.ToString());
    //    var invoiceAddress = await serviceManager.UserAddressService.GetByIdAsync<ResultUserAddressDto>(order.InvoiceAddressId.ToString());

    //    // Get order items
    //    var orderItemsQuery = serviceManager.OrderItemService
    //        .GetWhere<ResultOrderItemDto>(oi => oi.OrderId == orderId);

    //    var orderItems = orderItemsQuery.Data is not null
    //    ? await orderItemsQuery.Data.ToListAsync() : [];

    //    // Fetch all service details for each order item
    //    var services = new List<ResultServiceWithImageDto>();
    //    foreach (var item in orderItems)
    //    {
    //        var serviceResult = await serviceManager.ServiceService
    //            .GetServiceWithImagesByIdAsync<ResultServiceWithImageDto>(item.ServiceId.ToString(), false);

    //        if (serviceResult.IsSuccessful && serviceResult.Data is not null)
    //            services.Add(serviceResult.Data);
    //    }

    //    // Prepare and return model
    //    var model = new OrderDetailViewModel
    //    {
    //        Order = order,
    //        OrderItems = orderItems,
    //        Services = services,
    //        ShippingAddress = shippingAddress.Data ?? new ResultUserAddressDto(),
    //        InvoiceAddress = invoiceAddress.Data ?? new ResultUserAddressDto(),
    //    };

    //    return View(model);

    //    IViewComponentResult EmptyViewModel() => View(new OrderDetailViewModel());
    //}


    #endregion

    public async Task<IViewComponentResult> InvokeAsync(Guid orderId)
    {
        var stopwatch = Stopwatch.StartNew();
        var userResult = await serviceManager.SystemUserService.GetCurrentUserAsync();
        if (!userResult.IsSuccessful)
            return View(new ResultOrderDetailDto());

        var result = await serviceManager.OrderService.GetOrderDetailByIdAsync(orderId, userResult.Data.Id, false, true);

        stopwatch.Stop();
        var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
        Console.WriteLine($"Veri {elapsedSeconds} saniyede geldi.");

        return View(result.Data ?? new ResultOrderDetailDto());
    }
}
