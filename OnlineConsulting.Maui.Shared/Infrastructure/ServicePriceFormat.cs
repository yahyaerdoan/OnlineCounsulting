using OnlineConsulting.Maui.Shared.Infrastructure.Api;

namespace OnlineConsulting.Maui.Shared.Infrastructure;

/// <summary>Customer-facing price text for a service, honoring its price type (fixed, "from", range) and discount.</summary>
public static class ServicePriceFormat
{
    /// <summary>The price the customer pays: the discounted price for discounted fixed services, otherwise the list price text.</summary>
    public static string Display(ServiceResponse service) => service.PriceType switch
    {
        ServicePriceTypes.StartingAt => $"From {CurrencyFormat.Format(service.Price)}",
        ServicePriceTypes.Range when service.PriceMax is decimal max => $"{CurrencyFormat.Format(service.Price)} - {CurrencyFormat.Format(max)}",
        _ when service.DiscountRate > 0 => CurrencyFormat.Format(service.DiscountedPrice),
        _ => CurrencyFormat.Format(service.Price),
    };

    /// <summary>Whether the card should show a struck-through original price next to Display().</summary>
    public static bool HasDiscount(ServiceResponse service) =>
        service.DiscountRate > 0 && service.PriceType is not (ServicePriceTypes.StartingAt or ServicePriceTypes.Range);
}
