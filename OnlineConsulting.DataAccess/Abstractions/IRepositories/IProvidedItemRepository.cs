using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Abstractions.IRepositories;

public interface IProvidedItemRepository : IGenericRepository<ProvidedItem>
{
    IQueryable<ProvidedItem> GetAllProvidedItemsWithImgIcons(bool traking = true, bool? status = true);
}
