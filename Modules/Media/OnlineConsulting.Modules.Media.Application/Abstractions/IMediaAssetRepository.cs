using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Media.Domain;

namespace OnlineConsulting.Modules.Media.Application.Abstractions;

/// <summary>Repository for media asset records.</summary>
public interface IMediaAssetRepository : IAsyncRepository<MediaAsset, Guid>
{
}
