namespace OnlineConsulting.Modules.Commerce.Application.Common;

/// <summary>Single source of truth for the subtotal/tax/total formula shared by BasketItem and OrderItem handlers.</summary>
public static class TaxCalculator
{
    private const decimal PercentageDivisor = 100m;

    public static (decimal SubTotalPrice, decimal TaxAmount, decimal TotalPrice) Calculate(decimal unitPrice, int quantity, int taxRatePercent)
    {
        var subTotalPrice = unitPrice * quantity;
        var taxAmount = subTotalPrice * taxRatePercent / PercentageDivisor;
        return (subTotalPrice, taxAmount, subTotalPrice + taxAmount);
    }
}
