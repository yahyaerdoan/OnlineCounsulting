using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Repositories;

public class InvoiceLineRepository(CommerceDbContext context) : EfRepositoryBase<InvoiceLine, Guid, CommerceDbContext>(context), IInvoiceLineRepository
{
}
