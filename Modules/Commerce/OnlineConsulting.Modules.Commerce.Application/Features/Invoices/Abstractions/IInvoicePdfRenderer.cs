using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;

/// <summary>Turns an invoice into a printable PDF; the rendering library stays in Infrastructure.</summary>
public interface IInvoicePdfRenderer
{
    /// <summary>Dates are shown in <paramref name="timeZone"/>, the business's zone. A logo the renderer can't read is left out rather than failing the PDF.</summary>
    byte[] Render(InvoiceResponse invoice, InvoiceBusinessInfo business, TimeZoneInfo timeZone, byte[]? logo);
}
