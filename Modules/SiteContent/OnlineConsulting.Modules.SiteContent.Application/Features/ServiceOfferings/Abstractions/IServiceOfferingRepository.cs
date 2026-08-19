using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain.Service;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.Abstractions;

public interface IServiceOfferingRepository : IAsyncRepository<ServiceOffering, Guid>
{
}
