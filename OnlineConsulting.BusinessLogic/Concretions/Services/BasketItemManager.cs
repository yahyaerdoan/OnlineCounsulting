using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.BasketItemDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class BasketItemManager(IMapper mapper, IGenericRepository<BasketItem> repository, IServiceRepository serviceRepository, IBasketRepository basketRepository, IBasketItemRepository basketItemRepository, ICurrentUserAccessor currentUser, IBasketService basketService) : GenericService<BasketItem, IDto>(mapper, repository), IBasketItemService
{
    public async Task<IOperationResult> CreateBasketItemAsync(Guid serviceId, int quantity = 1)
    {
        var basketResult = await basketService.CreateBasketAsync();
        if (!basketResult.IsSuccessful || basketResult.Data is null)
            return new ErrorResult("You need to be logged in to add this item to your cart.", ResultStatus.BadRequest);

        var basketId = basketResult.Data.Id;

        var existingBasketItem = await _repository
            .GetSingleAsync(bi => bi.BasketId == basketId && bi.ServiceId == serviceId, false, null);

        if (existingBasketItem is not null)
        {
            existingBasketItem.Status = true;
            existingBasketItem.Quantity = quantity;
            existingBasketItem.SubTotalPrice = existingBasketItem.Quantity * existingBasketItem.Price;
            existingBasketItem.TaxAmount = (existingBasketItem.SubTotalPrice * existingBasketItem.TaxRate) / 100;
            existingBasketItem.TotalPrice = existingBasketItem.SubTotalPrice + existingBasketItem.TaxAmount;

            await _repository.UpdateAsync(existingBasketItem);
            await _repository.SaveAsync();

            await basketService.UpdateBasketTotalsAsync(basketId);

            return new SuccessResult("Basket item successfully added.", ResultStatus.Created);
        }

        var service = await serviceRepository.GetByIdAsync(serviceId.ToString(), false, true);
        if (service is null)
            return new ErrorResult("Service not found.", ResultStatus.BadRequest);

        var price = service.DiscountedPrice;
        var taxRate = (int)service.TaxRate;
        var subTotalPrice = price * quantity;
        var taxAmount = (subTotalPrice * taxRate) / 100;
        var totalPrice = subTotalPrice + taxAmount;

        var basketItemDto = new CreateBasketItemDto
        {
            BasketId = basketId,
            ServiceId = service.Id,
            Price = price,
            Quantity = quantity,
            TaxRate = taxRate,
            TaxAmount = taxAmount,
            SubTotalPrice = subTotalPrice,
            TotalPrice = totalPrice
        };

        var basketItemEntity = _mapper.Map<BasketItem>(basketItemDto);

        await _repository.AddAsync(basketItemEntity);
        await _repository.SaveAsync();

        await basketService.UpdateBasketTotalsAsync(basketId);

        return new SuccessResult("Basket item successfully added.", ResultStatus.Created);
    }
    public async Task<IOperationResult> RemoveBasketItemAsync(Guid userId, Guid basketItemId)
    {
        var basket = await basketRepository.GetBasketByUserIdAsync(userId, tracking: false, status: true);
        if (basket is null)
            return new ErrorResult("Basket not found.", ResultStatus.NotFound);

        var basketItem = await _repository.Entity
            .FirstOrDefaultAsync(bi => bi.Id == basketItemId && bi.BasketId == basket.Id);

        if (basketItem is null)
            return new ErrorResult("Basket item not found or doesn't belong to the user.", ResultStatus.NotFound);

        await _repository.RemoveAsync(basketItem);
        await _repository.SaveAsync();

        await basketService.UpdateBasketTotalsAsync(basket.Id);

        return new SuccessResult("Basket item removed successfully.");
    }
    public async Task<IOperationResult<IQueryable<ResultBasketItemDto>>> GetBasketItemsByUserIdAsync(Guid id, bool tracking = true, bool? status = true)
    {
        if (currentUser.UserId is not { } rawUserId || !Guid.TryParse(rawUserId, out var userId))
            return new ErrorDataResult<IQueryable<ResultBasketItemDto>>("User not found or not logged in.", ResultStatus.Unauthorized);

        var basket = await basketRepository.GetBasketByUserIdAsync(userId, tracking, status);

        if (basket is null)
        {
            return new ErrorDataResult<IQueryable<ResultBasketItemDto>>("Basket not found.", ResultStatus.BadRequest);
        }

        var basketItemsQuery = basketItemRepository.Entity.Where(bi => bi.BasketId == basket.Id);

        if (status.HasValue)
            basketItemsQuery = basketItemsQuery.Where(bi => bi.Status == status.Value);

        if (!tracking)
            basketItemsQuery = basketItemsQuery.AsNoTracking();

        var basketItemsDto = _mapper.ProjectTo<ResultBasketItemDto>(basketItemsQuery);

        return new SuccessDataResult<IQueryable<ResultBasketItemDto>>(basketItemsDto, "Basket items retrieved successfully.", ResultStatus.Ok);
    }
    /// <summary>TODO: decide whether to reuse the generic service's remove logic here instead.</summary>
    public async Task<IOperationResult> ClearBasketItemsByIdAsync(Guid basketId)
    {
        var items = await _repository.GetWhere(bi => bi.BasketId == basketId).ToListAsync();

        if (items.Count == 0)
            return new ErrorResult("No basket items found.", ResultStatus.NotFound);

        var removed = _repository.RemoveRange(items, false); // false = hard delete; true would soft-delete instead
        var saved = await _repository.SaveAsync();

        return removed && saved > 0
            ? new SuccessResult("Basket items cleared successfully.", ResultStatus.Ok)
            : new ErrorResult("Failed to clear basket items.", ResultStatus.InternalServerError);
    }
    public async Task<IOperationResult<int>> GetTotalBasketItemsCountAsync(bool tracking = true, bool? status = true)
    {
        if (currentUser.UserId is not { } rawUserId || !Guid.TryParse(rawUserId, out var userId))
        {
            return new ErrorDataResult<int>("User not found or not logged in.", ResultStatus.Unauthorized);
        }

        var basket = await basketRepository.GetBasketByUserIdAsync(userId, tracking, status);
        if (basket is null)
        {
            return new ErrorDataResult<int>("Basket not found or does not belong to the user.", ResultStatus.BadRequest);
        }

        var result = await basketItemRepository.GetTotalBasketItemsCountAsync(basket.Id, tracking, status);
        if (result < 0)
        {
            return new ErrorDataResult<int>("Failed to retrieve basket items count.", ResultStatus.BadRequest);
        }

        return new SuccessDataResult<int>(result, "Total basket items count retrieved successfully.", ResultStatus.Ok);
    }
}
