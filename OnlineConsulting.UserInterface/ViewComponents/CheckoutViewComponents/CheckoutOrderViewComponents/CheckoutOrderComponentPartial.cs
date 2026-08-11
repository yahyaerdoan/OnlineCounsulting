using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.BasketItemDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.CheckoutViewComponents.CheckoutOrderViewComponents;

public class CheckoutOrderComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(Guid cartId)
    {
        var result = await serviceManager.BasketItemService.GetBasketItemsByUserIdAsync(cartId.ToString(), false, true);
        return View(result.Data ?? Enumerable.Empty<ResultBasketItemDto>().AsQueryable());
    }
}
