using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Abstractions;
using OnlineConsulting.Modules.Equipment.Domain;
using OnlineConsulting.Modules.Equipment.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Equipment.Infrastructure.Repositories;

public class EquipmentItemRepository(EquipmentDbContext context) : EfRepositoryBase<EquipmentItem, Guid, EquipmentDbContext>(context), IEquipmentItemRepository
{
}
