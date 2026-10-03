namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;

public record PayInvoiceResult(bool Paid, string? ClientSecret);
