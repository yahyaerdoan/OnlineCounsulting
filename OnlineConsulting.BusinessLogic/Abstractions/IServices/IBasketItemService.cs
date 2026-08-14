using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.BasketItemDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IBasketItemService : IGenericService<BasketItem, IDto>
{
    Task<IOperationResult> CreateBasketItemAsync(Guid serviceId, int quantity = 1);
    Task<IOperationResult> RemoveBasketItemAsync(Guid userId, Guid basketItemId);
    Task<IOperationResult<IQueryable<ResultBasketItemDto>>> GetBasketItemsByUserIdAsync(Guid id, bool tracking = true, bool? status = true);
    Task<IOperationResult> ClearBasketItemsByIdAsync(Guid basketId);
    Task<IOperationResult<int>> GetTotalBasketItemsCountAsync(bool tracking = true, bool? status = true);
}
