using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Abstractions.IRepositories;

public interface ISocialMediaRepository : IGenericRepository<SocialMedia>
{
    IQueryable<SocialMedia> GetAllSocialMediaAccontsWithIcon(bool traking = true, bool? status = true);
}
