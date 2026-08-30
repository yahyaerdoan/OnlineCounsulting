namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors Equipment's EquipmentItemResponse.</summary>
public record EquipmentItemResponse(Guid Id, Guid UserId, string Type, string? Brand, string? Model, string? SerialNumber,
    DateTimeOffset? InstallDate, DateTimeOffset? WarrantyExpiresAt, string? Notes) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Type), nameof(Brand), nameof(Model), nameof(SerialNumber)];
}
