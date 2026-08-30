using System.Globalization;

namespace OnlineConsulting.Maui.Shared.Infrastructure;

/// <summary>Formats a price as "$X.XX" - explicit en-US currency, not the runtime's current culture,
/// so it always matches the literal "$" shown on every editable price field's adornment.</summary>
public static class CurrencyFormat
{
    private static readonly CultureInfo UsdCulture = CultureInfo.GetCultureInfo("en-US");

    public static string Format(decimal amount) => amount.ToString("C", UsdCulture);
}
