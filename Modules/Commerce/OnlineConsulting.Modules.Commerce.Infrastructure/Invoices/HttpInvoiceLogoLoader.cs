using Microsoft.Extensions.Logging;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Invoices;

/// <summary>Downloads over HTTP so it works for every storage backend; capped in size and time so a slow or huge file can't stall the PDF.</summary>
public sealed class HttpInvoiceLogoLoader(IHttpClientFactory httpClientFactory, ILogger<HttpInvoiceLogoLoader> logger) : IInvoiceLogoLoader
{
    public const string HttpClientName = "InvoiceLogo";
    private const long MaxBytes = 2 * 1024 * 1024;

    public async Task<byte[]?> LoadAsync(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await httpClientFactory.CreateClient(HttpClientName).GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode || response.Content.Headers.ContentLength > MaxBytes)
            {
                return null;
            }

            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            return bytes.Length > MaxBytes ? null : bytes;
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            logger.LogWarning(exception, "Could not load the invoice logo from {Url}.", url);
            return null;
        }
    }
}
