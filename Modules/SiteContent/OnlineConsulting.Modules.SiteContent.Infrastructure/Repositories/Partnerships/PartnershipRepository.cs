using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Partnerships;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories.Partnerships;

public class PartnershipRepository(SiteContentDbContext context) : EfRepositoryBase<Partnership, Guid, SiteContentDbContext>(context), IPartnershipRepository
{
}
