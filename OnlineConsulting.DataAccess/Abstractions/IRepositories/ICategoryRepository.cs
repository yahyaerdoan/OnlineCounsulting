using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Abstractions.IRepositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    IQueryable<Category> GetAllCategoriesWithImgIcons(bool traking = true, bool? status = true);
}
