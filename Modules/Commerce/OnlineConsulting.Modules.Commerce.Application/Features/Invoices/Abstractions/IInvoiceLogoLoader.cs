namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;

/// <summary>Fetches the business logo for the invoice PDF.</summary>
public interface IInvoiceLogoLoader
{
    /// <summary>The image bytes, or null when it can't be fetched; a missing logo never fails the PDF.</summary>
    Task<byte[]?> LoadAsync(string url, CancellationToken cancellationToken = default);
}
