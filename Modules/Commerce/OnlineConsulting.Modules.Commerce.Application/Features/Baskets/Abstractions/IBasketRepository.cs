using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;

public interface IBasketRepository : IAsyncRepository<Basket, Guid>
{
    /// <summary>The basket of a user or a guest (exactly one given), loaded with its items.</summary>
    Task<Basket?> GetForOwnerAsync(Guid? userId, Guid? guestId, bool enableTracking = true, CancellationToken cancellationToken = default);
}
