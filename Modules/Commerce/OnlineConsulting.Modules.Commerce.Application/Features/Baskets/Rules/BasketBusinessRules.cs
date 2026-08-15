using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Rules;

/// <summary>Named, reusable guard clauses instead of each handler repeating its own Result.NotFound literal.</summary>
public static class BasketBusinessRules
{
    public static OperationResult BasketNotFound() => Result.NotFound(BasketMessages.BasketNotFound);

    public static OperationResult BasketItemNotFound(Guid basketItemId) =>
        Result.NotFound(string.Format(BasketMessages.BasketItemNotFoundFormat, basketItemId));
}
