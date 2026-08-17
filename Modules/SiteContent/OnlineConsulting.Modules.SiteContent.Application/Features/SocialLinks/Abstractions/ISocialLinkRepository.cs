using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.Abstractions;

public interface ISocialLinkRepository : IAsyncRepository<SocialLink, Guid>
{
}
