using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain.Legal;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LegalPolicies.Abstractions;

public interface ILegalPolicyRepository : IAsyncRepository<LegalPolicy, Guid>
{
}
