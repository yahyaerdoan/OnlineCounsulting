using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Media.Application;
using OnlineConsulting.Modules.Media.Domain;
using OnlineConsulting.Modules.Media.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Media.Infrastructure.Repositories;

public class MediaAssetRepository(MediaDbContext context) : EfRepositoryBase<MediaAsset, Guid, MediaDbContext>(context), IMediaAssetRepository
{
}
