using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Rules;

// Pilot for the Commerce module: named, reusable guard clauses instead of each handler repeating
// its own Result.NotFound("Basket not found.") literal. Kept to plain OperationResult on purpose -
// the basket/item the caller already fetched stays in scope (and null-narrowed) after these calls,
// so there's no need to thread the entity back through the result the way a generic Data-carrying
// rule would.
public static class BasketBusinessRules
{
    public static OperationResult BasketNotFound() => Result.NotFound(BasketMessages.BasketNotFound);

    public static OperationResult BasketItemNotFound(Guid basketItemId) =>
        Result.NotFound(string.Format(BasketMessages.BasketItemNotFoundFormat, basketItemId));
}
