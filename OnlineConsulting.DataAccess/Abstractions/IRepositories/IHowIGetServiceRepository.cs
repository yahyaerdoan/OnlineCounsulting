using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Abstractions.IRepositories;

public interface IHowIGetServiceRepository : IGenericRepository<HowIGetService>
{
    IQueryable<HowIGetService> GetAllHowIGetServicesWithImgIcons(bool traking = true, bool? status = true);
}
