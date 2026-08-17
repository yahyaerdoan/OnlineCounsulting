using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.Abstractions;

public interface IPartnershipSocialLinkRepository : IAsyncRepository<PartnershipSocialLink, Guid>
{
}
