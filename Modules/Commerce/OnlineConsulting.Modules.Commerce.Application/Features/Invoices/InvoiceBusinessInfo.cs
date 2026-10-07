namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices;

/// <summary>Who an invoice is from: the tenant's brand name and logo, the details on its Contact page and its payment terms.</summary>
public sealed record InvoiceBusinessInfo(string BusinessName, string? LogoUrl, string? BusinessEmail, string? BusinessPhone, string? BusinessAddress, int PaymentTermsDays);
