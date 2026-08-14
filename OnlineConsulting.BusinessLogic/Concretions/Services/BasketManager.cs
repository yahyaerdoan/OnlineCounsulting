using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.BasketDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class BasketManager(IMapper mapper, IGenericRepository<Basket> repository, IBasketItemRepository basketItemRepository, ICurrentUserAccessor currentUser) : GenericService<Basket, IDto>(mapper, repository), IBasketService
{
    public async Task<IOperationResult<ResultBasketDto>> CreateBasketAsync()
    {
        if (currentUser.UserId is not { } rawUserId || !Guid.TryParse(rawUserId, out var userId))
        {
            return new ErrorDataResult<ResultBasketDto>($"You must log in to add services to your cart. Please log in and try again.", ResultStatus.BadRequest);
        }

        var existingBasket = await _repository.GetWhere(b => b.UserId == userId).FirstOrDefaultAsync();
        if (existingBasket is not null)
        {
            var existingBasketDto = _mapper.Map<ResultBasketDto>(existingBasket);
            return new SuccessDataResult<ResultBasketDto>(existingBasketDto, "Existing basket returned.", ResultStatus.Ok);
        }

        var basketDto = new CreateBasketDto
        {
            UserId = userId,
            Quantity = 0,
            TotalPrice = 0,
            SubTotalPrice = 0
        };

        var basket = _mapper.Map<Basket>(basketDto);

        await _repository.AddAsync(basket);
        await _repository.SaveAsync();

        var resultBasketDto = _mapper.Map<ResultBasketDto>(basket);

        return new SuccessDataResult<ResultBasketDto>(resultBasketDto, "Basket is created successfully.", ResultStatus.Created);
    }

    public async Task<IOperationResult> DeleteBasketByIdAsync(Guid basketId)
    {
        var result = await RemoveByIdAsync(basketId.ToString(), false);
        return result;
    }
    public async Task UpdateBasketTotalsAsync(Guid basketId)
    {
        // Get all basket items for the current basket
        var basketItems = await basketItemRepository.GetWhere(bi => bi.BasketId == basketId).ToListAsync();

        if (basketItems is null || basketItems.Count == 0)
            return;

        // Calculate totals
        var totalQuantity = basketItems.Sum(bi => bi.Quantity);
        var subTotal = basketItems.Sum(bi => bi.SubTotalPrice);
        var totalPrice = basketItems.Sum(bi => bi.TotalPrice);

        // Retrieve the Basket entity to update
        var basket = await _repository.GetSingleAsync(b => b.Id == basketId);
        if (basket is null)
            return;

        basket.Quantity = totalQuantity;
        basket.SubTotalPrice = basketItems.Sum(bi => bi.SubTotalPrice);
        basket.TotalPrice = basketItems.Sum(bi => bi.TotalPrice);

        await _repository.UpdateAsync(basket);
        await _repository.SaveAsync();
    }
}
