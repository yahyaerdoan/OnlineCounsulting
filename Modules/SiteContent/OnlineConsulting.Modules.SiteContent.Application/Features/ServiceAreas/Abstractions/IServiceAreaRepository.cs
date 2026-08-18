using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Abstractions;

public interface IServiceAreaRepository : IAsyncRepository<ServiceArea, Guid>
{
}
