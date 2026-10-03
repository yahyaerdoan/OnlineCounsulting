using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;

/// <summary>Turns an invoice into a printable PDF; the rendering library stays in Infrastructure.</summary>
public interface IInvoicePdfRenderer
{
    byte[] Render(InvoiceResponse invoice, InvoiceBusinessInfo business);
}
