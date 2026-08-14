namespace OnlineConsulting.Modules.Services.Application.Common;

// The legacy MVC stack trusted the caller to supply DiscountedPrice directly (CreateServiceDto had
// a plain settable field). Computing it server-side instead removes a client-trusted money field -
// the caller only ever supplies Price/DiscountRate, never the derived price.
public static class ServicePriceCalculator
{
    private const decimal PercentageDivisor = 100m;

    public static decimal CalculateDiscountedPrice(decimal price, int discountRatePercent) =>
        price - (price * discountRatePercent / PercentageDivisor);
}
