using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.SiteContent.Application.Features.LegalPolicies.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Legal;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Repositories.Legal;

public class LegalPolicyRepository(SiteContentDbContext context) : EfRepositoryBase<LegalPolicy, Guid, SiteContentDbContext>(context), ILegalPolicyRepository
{
}
