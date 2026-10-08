namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;

/// <summary>The caller's business's invoicing preferences; the defaults when it never changed them.</summary>
public record InvoiceSettingsResponse(int PaymentTermsDays);
