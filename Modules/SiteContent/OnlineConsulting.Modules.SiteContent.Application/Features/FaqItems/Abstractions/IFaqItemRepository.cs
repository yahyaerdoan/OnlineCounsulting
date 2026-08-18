using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.Abstractions;

public interface IFaqItemRepository : IAsyncRepository<FaqItem, Guid>
{
}
