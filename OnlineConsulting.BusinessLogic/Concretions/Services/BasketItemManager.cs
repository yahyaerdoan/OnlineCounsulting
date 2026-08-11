using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.BasketItemDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class BasketItemManager(IMapper mapper, IGenericRepository<BasketItem> repository, IServiceRepository serviceRepository, IBasketRepository basketRepository, IBasketItemRepository basketItemRepository, ISystemUserService systemUserService, IBasketService basketService) : GenericService<BasketItem, IDto>(mapper, repository), IBasketItemService
{
    public async Task<IOperationResult> CreateBasketItemAsync(Guid serviceId, int quantity = 1)
    {
        // Step 1: Get or create user's basket
        var basketResult = await basketService.CreateBasketAsync();
        if (!basketResult.IsSuccessful || basketResult.Data is null)
            return new ErrorResult("You need to be logged in to add this item to your cart.", ResultStatus.BadRequest);

        var basketId = basketResult.Data.Id;

        // Step 2: Check if basket item already exists
        var existingBasketItem = await _repository
            .GetSingleAsync(bi => bi.BasketId == basketId && bi.ServiceId == serviceId, false, null);

        if (existingBasketItem is not null)
        {
            // Update existing quantity
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

        // Step 3: Get service details if item is new
        var service = await serviceRepository.GetByIdAsync(serviceId.ToString(), false, true);
        if (service is null)
            return new ErrorResult("Service not found.", ResultStatus.BadRequest);

        // Step 3a: Calculate Basket Item details
        var price = service.DiscountedPrice;
        var taxRate = (int)service.TaxRate;
        var subTotalPrice = price * quantity;
        var taxAmount = (subTotalPrice * taxRate) / 100;
        var totalPrice = subTotalPrice + taxAmount;

        // Step 4: Create BasketItemDto
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

        // Step 4: Map and save
        var basketItemEntity = _mapper.Map<BasketItem>(basketItemDto);

        await _repository.AddAsync(basketItemEntity);
        await _repository.SaveAsync();

        await basketService.UpdateBasketTotalsAsync(basketId);

        return new SuccessResult("Basket item successfully added.", ResultStatus.Created);
    }
    public async Task<IOperationResult> RemoveBasketItemAsync(string userId, Guid basketItemId)
    {
        // 1. Get user's current basket
        var basket = await basketRepository.GetBasketByUserIdAsync(userId, tracking: false, status: true);
        if (basket is null)
            return new ErrorResult("Basket not found.", ResultStatus.NotFound);

        // 2. Find the basket item inside that basket
        var basketItem = await _repository.Entity
            .FirstOrDefaultAsync(bi => bi.Id == basketItemId && bi.BasketId == basket.Id);

        if (basketItem is null)
            return new ErrorResult("Basket item not found or doesn't belong to the user.", ResultStatus.NotFound);

        // 3. Remove item
        await _repository.RemoveAsync(basketItem);
        await _repository.SaveAsync();

        await basketService.UpdateBasketTotalsAsync(basket.Id);

        return new SuccessResult("Basket item removed successfully.");
    }
    public async Task<IOperationResult<IQueryable<ResultBasketItemDto>>> GetBasketItemsByUserIdAsync(string id, bool tracking = true, bool? status = true)
    {
        var userResult = await systemUserService.GetCurrentUserAsync();
        if (!userResult.IsSuccessful || userResult.Data is null)
            return new ErrorDataResult<IQueryable<ResultBasketItemDto>>("User not found or not logged in.", ResultStatus.Unauthorized);

        var userId = userResult.Data.Id;
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
    //TODO: Check this code from generic service, to use it or not
    public async Task<IOperationResult> ClearBasketItemsByIdAsync(Guid basketId)
    {
        var items = await _repository.GetWhere(bi => bi.BasketId == basketId).ToListAsync();

        if (items.Count == 0)
            return new ErrorResult("No basket items found.", ResultStatus.NotFound);

        var removed = _repository.RemoveRange(items, false); // or true for soft delete
        var saved = await _repository.SaveAsync();

        return removed && saved > 0
            ? new SuccessResult("Basket items cleared successfully.", ResultStatus.Ok)
            : new ErrorResult("Failed to clear basket items.", ResultStatus.InternalServerError);
    }
    public async Task<IOperationResult<int>> GetTotalBasketItemsCountAsync(bool tracking = true, bool? status = true)
    {
        var userResult = await systemUserService.GetCurrentUserAsync();

        // Check if ResultData is null
        if (userResult is null || userResult.Data is null || string.IsNullOrEmpty(userResult.Data.Id))
        {
            return new ErrorDataResult<int>("User not found or not logged in.", ResultStatus.Unauthorized);
        }

        var userId = userResult.Data.Id;

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
