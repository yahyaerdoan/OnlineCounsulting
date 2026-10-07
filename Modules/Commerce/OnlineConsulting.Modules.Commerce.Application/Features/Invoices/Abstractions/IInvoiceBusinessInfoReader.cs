namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;

/// <summary>Assembles who a tenant's invoices are from.</summary>
public interface IInvoiceBusinessInfoReader
{
    Task<InvoiceBusinessInfo> GetAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
