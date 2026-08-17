using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Services.Domain;

namespace OnlineConsulting.Modules.Services.Application.Features.ServiceMediaItems.Abstractions;

public interface IServiceMediaItemRepository : IAsyncRepository<ServiceMediaItem, Guid>
{
}
