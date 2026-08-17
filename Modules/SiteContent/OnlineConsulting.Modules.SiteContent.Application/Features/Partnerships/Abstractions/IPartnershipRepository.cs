using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.Abstractions;

public interface IPartnershipRepository : IAsyncRepository<Partnership, Guid>
{
}
