using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Equipment;

public record EquipmentListItemViewModel(
    Guid Id,
    string CustomerName,
    string Type,
    string? Brand,
    string? Model,
    string? SerialNumber,
    DateTimeOffset? InstallDate,
    DateTimeOffset? WarrantyExpiresAt,
    string? Notes);

public record CustomerOptionViewModel(Guid Id, string Name);

public class CreateEquipmentViewModel
{
    [Required]
    public Guid CustomerUserId { get; set; }

    [Required, MaxLength(100)]
    public string Type { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Brand { get; set; }

    [MaxLength(100)]
    public string? Model { get; set; }

    [MaxLength(100)]
    public string? SerialNumber { get; set; }

    public DateOnly? InstallDate { get; set; }

    public DateOnly? WarrantyExpiresAt { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    public List<CustomerOptionViewModel> Customers { get; set; } = [];
}

public class UpdateEquipmentViewModel
{
    public Guid Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Type { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Brand { get; set; }

    [MaxLength(100)]
    public string? Model { get; set; }

    [MaxLength(100)]
    public string? SerialNumber { get; set; }

    public DateOnly? InstallDate { get; set; }

    public DateOnly? WarrantyExpiresAt { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }
}
