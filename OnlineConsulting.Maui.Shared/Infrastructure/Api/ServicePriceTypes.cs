namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors Modules.Services.Application.Features.Constants.ServicePriceTypes.</summary>
public static class ServicePriceTypes
{
    public const string Fixed = "Fixed";
    public const string StartingAt = "StartingAt";
    public const string Range = "Range";

    public static readonly string[] All = [Fixed, StartingAt, Range];
}
