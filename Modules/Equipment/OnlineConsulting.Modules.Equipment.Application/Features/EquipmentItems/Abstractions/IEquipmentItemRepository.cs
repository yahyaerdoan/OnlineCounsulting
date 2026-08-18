using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Equipment.Domain;

namespace OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Abstractions;

public interface IEquipmentItemRepository : IAsyncRepository<EquipmentItem, Guid>
{
}
