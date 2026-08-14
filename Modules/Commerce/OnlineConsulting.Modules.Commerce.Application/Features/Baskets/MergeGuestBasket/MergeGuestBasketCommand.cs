using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Contracts;
using OnlineConsulting.Modules.Commerce.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.MergeGuestBasket;

// Called from the Api layer right after a successful login (not ISecureAddRequest - it runs in
// the same request that just authenticated the user, before any Commerce-side auth check would
// see the new token). If there's no guest basket, this is a no-op success - most logins won't
// have shopped as a guest first.
public record MergeGuestBasketCommand(Guid UserId, Guid GuestId) : IRequest<OperationResult>, ITransactionAddRequest;

public class MergeGuestBasketHandler(IBasketRepository basketRepository, IBasketItemRepository basketItemRepository)
    : IRequestHandler<MergeGuestBasketCommand, OperationResult>
{
    public async Task<OperationResult> Handle(MergeGuestBasketCommand request, CancellationToken cancellationToken)
    {
        var guestBasket = await basketRepository.GetAsync(b => b.GuestId == request.GuestId, cancellationToken: cancellationToken);
        if (guestBasket is null)
            return Result.Success("No guest basket to merge.");

        var guestItems = await basketItemRepository.GetListAsync(i => i.BasketId == guestBasket.Id, size: int.MaxValue, cancellationToken: cancellationToken);

        var userBasket = await basketRepository.GetAsync(b => b.UserId == request.UserId, cancellationToken: cancellationToken);
        if (userBasket is null)
        {
            userBasket = new Basket { Id = Guid.NewGuid(), UserId = request.UserId };
            await basketRepository.AddAsync(userBasket);
        }

        var userItems = await basketItemRepository.GetListAsync(i => i.BasketId == userBasket.Id, size: int.MaxValue, cancellationToken: cancellationToken);
        var userItemsByService = userItems.Items.ToDictionary(i => i.ServiceId);

        // Two separate sessions' quantities are combined (additive), unlike AddBasketItem's
        // same-session "re-adding overwrites quantity" behavior - merging a guest cart isn't the
        // same action as the user deliberately re-entering a quantity.
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

        var mergedItems = await basketItemRepository.GetListAsync(i => i.BasketId == userBasket.Id, size: int.MaxValue, cancellationToken: cancellationToken);
        (userBasket.Quantity, userBasket.SubTotalPrice, userBasket.TotalPrice) = BasketTotalsCalculator.Calculate(mergedItems.Items);
        await basketRepository.UpdateAsync(userBasket);

        return Result.Success("Guest basket merged successfully.");
    }
}
