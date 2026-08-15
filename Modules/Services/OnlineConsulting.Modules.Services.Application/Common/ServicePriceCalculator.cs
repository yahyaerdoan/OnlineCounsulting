namespace OnlineConsulting.Modules.Services.Application.Common;

/// <summary>Computes the discounted price server-side so callers can never supply a client-trusted money field directly.</summary>
public static class ServicePriceCalculator
{
    private const decimal PercentageDivisor = 100m;

    public static decimal CalculateDiscountedPrice(decimal price, int discountRatePercent) =>
        price - (price * discountRatePercent / PercentageDivisor);
}
