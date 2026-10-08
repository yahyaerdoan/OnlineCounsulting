namespace OnlineConsulting.Modules.Services.Domain;

/// <summary>How a service price is shown: Fixed as-is, StartingAt as "From {Price}", Range as "{Price} - {PriceMax}".</summary>
public static class ServicePriceTypes
{
    public const string Fixed = "Fixed";
    public const string StartingAt = "StartingAt";
    public const string Range = "Range";

    public static readonly string[] All = [Fixed, StartingAt, Range];
}
