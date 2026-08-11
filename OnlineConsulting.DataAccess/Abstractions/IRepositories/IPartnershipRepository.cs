using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Abstractions.IRepositories;

public interface IPartnershipRepository : IGenericRepository<Partnership>
{
    IQueryable<Partnership> GetAllPartnershipsWithSocialMedias(bool tracking = true, bool? status = true);
}
