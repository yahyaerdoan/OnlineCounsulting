using System.Globalization;

namespace OnlineConsulting.Maui.Shared.Infrastructure;

/// <summary>Formats a price as "$X.XX" - fixed en-US, not the runtime's culture.</summary>
public static class CurrencyFormat
{
    private static readonly CultureInfo UsdCulture = CultureInfo.GetCultureInfo("en-US");

    public static string Format(decimal amount) => amount.ToString("C", UsdCulture);
}
