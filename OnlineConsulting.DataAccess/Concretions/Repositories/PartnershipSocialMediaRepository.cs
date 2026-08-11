using Microsoft.EntityFrameworkCore;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataAccess.Concretions.Contexts;
using OnlineConsulting.DataAccess.Concretions.GenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Concretions.Repositories;

public class PartnershipSocialMediaRepository(OnlineConsultingDbContext context) : GenericRepository<PartnershipSocialMedia>(context), IPartnershipSocialMediRepository
{
    public IQueryable<PartnershipSocialMedia> GetAllSocialMediasByParnershipId(string id, bool traking = true, bool? status = true)
    {
        var query = Entity.Where(x => x.PartnershipId == Guid.Parse(id)).Include(x => x.ClassIcon).AsQueryable();
        if (!traking)
            query = query.AsNoTracking();
        if (status.HasValue)
            query = query.Where(e => e.Status == status);

        return query;
    }
}
