using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Common;

/// <summary>Applies the line formula (<see cref="LineAmounts"/>) to order items.</summary>
public static class TaxCalculator
{
    /// <summary>Recomputes an OrderItem's SubTotalPrice, TaxAmount and TotalPrice after its price, quantity or tax rate changes.</summary>
    public static void Apply(OrderItem item) =>
        (item.SubTotalPrice, item.TaxAmount, item.TotalPrice) = LineAmounts.Calculate(item.UnitPrice, item.Quantity, item.TaxRate);
}
