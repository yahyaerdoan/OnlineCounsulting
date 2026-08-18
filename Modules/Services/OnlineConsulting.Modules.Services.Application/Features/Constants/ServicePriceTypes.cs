namespace OnlineConsulting.Modules.Services.Application.Features.Constants;

public static class ServicePriceTypes
{
    public const string Fixed = "Fixed";
    public const string StartingAt = "StartingAt";
    public const string Range = "Range";

    public static readonly string[] All = [Fixed, StartingAt, Range];
}
