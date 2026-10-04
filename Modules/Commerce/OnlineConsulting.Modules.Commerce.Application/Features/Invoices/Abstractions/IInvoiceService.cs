using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;

/// <summary>Issues and settles invoices for both sources. Issuing is idempotent per source (a repeated webhook or retry returns the existing
/// invoice), and emails/pushes are best effort: a delivery failure is logged, never undoes the invoice.</summary>
public interface IInvoiceService
{
    Task<Invoice> IssueForPaidOrderAsync(Order order, IReadOnlyList<OrderItem> items, CancellationToken cancellationToken = default);

    /// <summary>Settles the invoice and sends the receipt; load it with <see cref="IInvoiceRepository.GetWithLinesAsync"/> so the receipt lists its lines.</summary>
    Task MarkPaidAsync(Invoice invoice, string paymentMethod, string? paymentProvider, string? providerPaymentId, CancellationToken cancellationToken = default);

    /// <summary>Voids an open invoice and tells the customer nothing is owed for it; load it with <see cref="IInvoiceRepository.GetWithLinesAsync"/>.</summary>
    Task VoidAsync(Invoice invoice, string? reason, CancellationToken cancellationToken = default);

    string? ViewUrl(Guid invoiceId);
}
