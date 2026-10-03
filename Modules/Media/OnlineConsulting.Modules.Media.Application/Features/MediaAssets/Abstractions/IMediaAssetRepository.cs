using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Media.Domain;

namespace OnlineConsulting.Modules.Media.Application.Features.MediaAssets.Abstractions;

/// <summary>Repository for media asset records.</summary>
public interface IMediaAssetRepository : IAsyncRepository<MediaAsset, Guid>
{
}
