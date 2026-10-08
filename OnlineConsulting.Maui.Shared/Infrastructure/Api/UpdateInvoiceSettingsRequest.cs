namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Body of PUT /api/v1/invoices/settings.</summary>
public record UpdateInvoiceSettingsRequest(int PaymentTermsDays);
