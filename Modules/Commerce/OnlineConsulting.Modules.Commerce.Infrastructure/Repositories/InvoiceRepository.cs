using System.Linq.Expressions;
using Core.PersistenceLayer.Repositories.EfRepositories;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Repositories;

public class InvoiceRepository(CommerceDbContext context) : EfRepositoryBase<Invoice, Guid, CommerceDbContext>(context), IInvoiceRepository
{
    public Task<Invoice?> GetWithLinesAsync(Expression<Func<Invoice, bool>> predicate, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        var query = Context.Invoices.Include(i => i.Lines).AsQueryable();
        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(predicate, cancellationToken);
    }
}
