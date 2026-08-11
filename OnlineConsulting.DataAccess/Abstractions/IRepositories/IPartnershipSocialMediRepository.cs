using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Abstractions.IRepositories;

public interface IPartnershipSocialMediRepository : IGenericRepository<PartnershipSocialMedia>
{
    IQueryable<PartnershipSocialMedia> GetAllSocialMediasByParnershipId(string id, bool traking = true, bool? status = true);
}
