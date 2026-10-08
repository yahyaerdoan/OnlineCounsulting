using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/v1/invoices/settings.</summary>
public record InvoiceSettingsResponse(int PaymentTermsDays) : HalResource;
