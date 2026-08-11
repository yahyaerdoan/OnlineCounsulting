using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.ViewComponents.CheckoutViewComponents.CheckoutCouponViewCmponents;

public class CheckoutCouponComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
