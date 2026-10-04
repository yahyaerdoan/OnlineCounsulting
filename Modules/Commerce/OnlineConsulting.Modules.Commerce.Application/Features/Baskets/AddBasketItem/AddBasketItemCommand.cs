using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Constants;
using OnlineConsulting.SharedKernel.Catalog;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.AddBasketItem;

/// <summary>Adds a product to the caller's (user or guest) basket at its catalog price; works for anonymous guests too.</summary>
public record AddBasketItemCommand(Guid? UserId, Guid? GuestId, Guid ServiceId, int Quantity)
    : IRequest<OperationResult>, ITransactionAddRequest;

public class AddBasketItemHandler(IBasketRepository basketRepository, IServiceCatalogReader catalogReader) : IRequestHandler<AddBasketItemCommand, OperationResult>
{
    public async Task<OperationResult> Handle(AddBasketItemCommand request, CancellationToken cancellationToken)
    {
        var catalogEntry = await catalogReader.GetAsync(request.ServiceId, cancellationToken);
        if (catalogEntry is null)
        {
            return Result.NotFound(BasketMessages.ServiceNotFound);
        }

        if (catalogEntry.Kind != ServiceKinds.Product)
        {
            return Result.BadRequest(BasketMessages.ServiceIsBookedNotBought);
        }

        var basket = await BasketOwnerLookup.GetOrOpenAsync(basketRepository, request.UserId, request.GuestId, cancellationToken);
        basket.AddItem(request.ServiceId, request.Quantity, catalogEntry.UnitPrice, catalogEntry.TaxRate);
        _ = await basketRepository.UpdateAsync(basket);

        return Result.Created("Basket item added successfully.");
    }
}
