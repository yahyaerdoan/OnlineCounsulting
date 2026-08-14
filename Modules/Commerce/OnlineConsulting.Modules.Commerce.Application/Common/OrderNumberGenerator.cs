namespace OnlineConsulting.Modules.Commerce.Application.Common;

public static class OrderNumberGenerator
{
    private const string Prefix = "ORD-";
    private const int RandomSegmentLength = 8;

    public static string Generate() => $"{Prefix}{Guid.NewGuid().ToString()[..RandomSegmentLength].ToUpperInvariant()}";
}
