namespace OnlineConsulting.UserInterface.Areas.User.Features.Equipment;

public record UserWorkOrderHistoryItemViewModel(Guid Id, DateTimeOffset? CompletedAt, string? PartsUsed, string? TechnicianNotes);

public record UserEquipmentListItemViewModel(
    Guid Id,
    string Type,
    string? Brand,
    string? Model,
    string? SerialNumber,
    DateTimeOffset? InstallDate,
    DateTimeOffset? WarrantyExpiresAt,
    string? Notes,
    List<UserWorkOrderHistoryItemViewModel> ServiceHistory);
