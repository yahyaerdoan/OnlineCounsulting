using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Media.Domain;

namespace OnlineConsulting.Modules.Media.Application.Abstractions;

public interface IMediaAssetRepository : IAsyncRepository<MediaAsset, Guid>
{
}
