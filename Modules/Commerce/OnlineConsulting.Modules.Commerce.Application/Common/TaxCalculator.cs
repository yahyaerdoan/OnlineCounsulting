using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Common;

/// <summary>Single source of truth for the subtotal/tax/total formula shared by BasketItem and OrderItem handlers.</summary>
public static class TaxCalculator
{
    private const decimal _percentageDivisor = 100m;

    public static (decimal SubTotalPrice, decimal TaxAmount, decimal TotalPrice) Calculate(decimal unitPrice, int quantity, int taxRatePercent)
    {
        var subTotalPrice = unitPrice * quantity;
        var taxAmount = subTotalPrice * taxRatePercent / _percentageDivisor;
        return (subTotalPrice, taxAmount, subTotalPrice + taxAmount);
    }

    /// <summary>Recomputes and writes back a BasketItem's own SubTotalPrice/TaxAmount/TotalPrice - call after any change to Price/Quantity/TaxRate.</summary>
    public static void Apply(BasketItem item) =>
        (item.SubTotalPrice, item.TaxAmount, item.TotalPrice) = Calculate(item.Price, item.Quantity, item.TaxRate);

    /// <summary>Recomputes and writes back an OrderItem's own SubTotalPrice/TaxAmount/TotalPrice - call after any change to UnitPrice/Quantity/TaxRate.</summary>
    public static void Apply(OrderItem item) =>
        (item.SubTotalPrice, item.TaxAmount, item.TotalPrice) = Calculate(item.UnitPrice, item.Quantity, item.TaxRate);
}
