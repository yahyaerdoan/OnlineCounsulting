using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.MergeGuestBasket;

/// <summary>Called right after login, before any Commerce-side auth check would see the new token, so this is deliberately not ISecureAddRequest.</summary>
public record MergeGuestBasketCommand(Guid UserId, Guid GuestId) : IRequest<OperationResult>, ITransactionAddRequest;

public class MergeGuestBasketHandler(IBasketRepository basketRepository, IBasketItemRepository basketItemRepository)
    : IRequestHandler<MergeGuestBasketCommand, OperationResult>
{
    public async Task<OperationResult> Handle(MergeGuestBasketCommand request, CancellationToken cancellationToken)
    {
        var guestBasket = await basketRepository.GetAsync(b => b.GuestId == request.GuestId, cancellationToken: cancellationToken);
        if (guestBasket is null)
            return Result.Success("No guest basket to merge.");

        var guestItems = await basketItemRepository.GetListAsync(i => i.BasketId == guestBasket.Id, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        var userBasket = await basketRepository.GetAsync(b => b.UserId == request.UserId, cancellationToken: cancellationToken);
        if (userBasket is null)
        {
            userBasket = new Basket { Id = Guid.NewGuid(), UserId = request.UserId };
            await basketRepository.AddAsync(userBasket);
        }

        var userItems = await basketItemRepository.GetListAsync(i => i.BasketId == userBasket.Id, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var userItemsByService = userItems.Items.ToDictionary(i => i.ServiceId);

        // Quantities are combined (additive) here, unlike AddBasketItem's same-session "re-adding overwrites quantity" behavior.
        foreach (var guestItem in guestItems.Items)
        {
            if (userItemsByService.TryGetValue(guestItem.ServiceId, out var existingItem))
            {
                existingItem.Quantity += guestItem.Quantity;
                (existingItem.SubTotalPrice, existingItem.TaxAmount, existingItem.TotalPrice) =
                    TaxCalculator.Calculate(existingItem.Price, existingItem.Quantity, existingItem.TaxRate);
                await basketItemRepository.UpdateAsync(existingItem);
            }
            else
            {
                var (subTotalPrice, taxAmount, totalPrice) = TaxCalculator.Calculate(guestItem.Price, guestItem.Quantity, guestItem.TaxRate);
                await basketItemRepository.AddAsync(new BasketItem
                {
                    Id = Guid.NewGuid(),
                    BasketId = userBasket.Id,
                    ServiceId = guestItem.ServiceId,
                    Quantity = guestItem.Quantity,
                    Price = guestItem.Price,
                    TaxRate = guestItem.TaxRate,
                    SubTotalPrice = subTotalPrice,
                    TaxAmount = taxAmount,
                    TotalPrice = totalPrice,
                });
            }

            await basketItemRepository.DeleteAsync(guestItem);
        }

        await basketRepository.DeleteAsync(guestBasket);

        var mergedItems = await basketItemRepository.GetListAsync(i => i.BasketId == userBasket.Id, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        (userBasket.Quantity, userBasket.SubTotalPrice, userBasket.TotalPrice) = BasketTotalsCalculator.Calculate(mergedItems.Items);
        await basketRepository.UpdateAsync(userBasket);

        return Result.Success("Guest basket merged successfully.");
    }
}
