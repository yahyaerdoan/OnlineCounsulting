using Microsoft.EntityFrameworkCore;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataAccess.Concretions.Contexts;
using OnlineConsulting.DataAccess.Concretions.GenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Concretions.Repositories;

public class PartnershipRepository(OnlineConsultingDbContext context) : GenericRepository<Partnership>(context), IPartnershipRepository
{
    public IQueryable<Partnership> GetAllPartnershipsWithSocialMedias(bool tracking = true, bool? status = true)
    {
        var query = Entity.Include(x => x.PartnershipSocialMedias).ThenInclude(x => x.ClassIcon).AsQueryable();
        if (!tracking)
            query = query.AsNoTracking();
        if (status.HasValue)
            query = query.Where(e => e.Status == status);

        return query;
    }
}
