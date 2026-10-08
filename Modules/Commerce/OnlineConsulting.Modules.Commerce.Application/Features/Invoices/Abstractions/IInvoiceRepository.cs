using System.Linq.Expressions;
using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;

public interface IInvoiceRepository : IAsyncRepository<Invoice, Guid>
{
    /// <summary>The first matching invoice loaded with its lines, for showing it or changing its state.</summary>
    Task<Invoice?> GetWithLinesAsync(Expression<Func<Invoice, bool>> predicate, bool enableTracking = true, CancellationToken cancellationToken = default);
}
