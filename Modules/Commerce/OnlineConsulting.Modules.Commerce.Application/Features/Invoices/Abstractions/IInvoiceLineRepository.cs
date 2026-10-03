using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;

public interface IInvoiceLineRepository : IAsyncRepository<InvoiceLine, Guid>
{
}
