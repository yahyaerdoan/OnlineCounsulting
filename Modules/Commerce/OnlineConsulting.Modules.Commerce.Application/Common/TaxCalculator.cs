namespace OnlineConsulting.Modules.Commerce.Application.Common;

// Single source of truth for the subtotal/tax/total formula shared by BasketItem and OrderItem
// handlers - keeps the two line-item types from drifting apart and keeps the percentage divisor
// out of handler code as a magic number. Domain stays anemic (plain entities, no behavior),
// matching this project's established convention - this is Application-layer, not a domain service.
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
