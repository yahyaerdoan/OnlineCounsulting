using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;

public interface IInvoiceRepository : IAsyncRepository<Invoice, Guid>
{
}
