using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Abstractions.IRepositories;

public interface IServiceRepository : IGenericRepository<Service>
{
    IQueryable<Service> GetAllServicesWithImages(bool traking = true, bool? status = true);
    IQueryable<Service> GetAllServicesByFeaturedAreaTrue(bool traking = true, bool? status = true);
    IQueryable<Service> GetAllServicesWithPaged(int size, int page, bool traking = true, bool? status = true);
    Task<Service?> GetServiceWithImagesByIdAsync(string id, bool traking = true, bool? status = true);
    IQueryable<Service> GetServicesWithImagesByCategoryIdAsync(string id, bool tracking = true, bool? status = true);
    Task<Service?> GetServiceBySlugWithImagesAndCategorydAsync(string slug, bool traking = true, bool? status = true);
    IQueryable<Service> SearchServicesWithImages(string query, bool traking = true, bool? status = true);
}
