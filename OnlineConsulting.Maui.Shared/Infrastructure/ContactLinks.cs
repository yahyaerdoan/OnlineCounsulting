namespace OnlineConsulting.Maui.Shared.Infrastructure;

/// <summary>Deep links for the company contact record, so taps open the phone's maps, dialer or mail app.</summary>
public static class ContactLinks
{
    /// <summary>Google Maps search for the address (opens the Maps app on phones); null when there is no address.</summary>
    public static string? Directions(string? address) =>
        string.IsNullOrWhiteSpace(address) ? null : $"https://www.google.com/maps/search/?api=1&query={Uri.EscapeDataString(address)}";
}
