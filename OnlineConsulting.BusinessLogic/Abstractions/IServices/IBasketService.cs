using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.BasketDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IBasketService : IGenericService<Basket, IDto>
{
    Task<IOperationResult<ResultBasketDto>> CreateBasketAsync();
    Task UpdateBasketTotalsAsync(Guid basketId);

    Task<IOperationResult> DeleteBasketByIdAsync(Guid id);
}
