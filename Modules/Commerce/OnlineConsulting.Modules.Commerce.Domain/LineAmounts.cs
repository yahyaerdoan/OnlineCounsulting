namespace OnlineConsulting.Modules.Commerce.Domain;

/// <summary>The subtotal, tax and total of one basket or order line.</summary>
public static class LineAmounts
{
    public static (decimal SubTotalPrice, decimal TaxAmount, decimal TotalPrice) Calculate(decimal unitPrice, int quantity, int taxRatePercent)
    {
        var subTotalPrice = unitPrice * quantity;
        var taxAmount = subTotalPrice * taxRatePercent / 100m;
        return (subTotalPrice, taxAmount, subTotalPrice + taxAmount);
    }
}
