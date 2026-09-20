using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using System.Linq.Expressions;

namespace OnlineConsulting.Modules.Commerce.Application.Common;

/// <summary>A user has at most one shipping and one billing address - claiming either flag unsets whichever other address currently holds it.</summary>
public static class UserAddressDefaultFlag
{
    public static Task ClearPreviousShippingHolderAsync(IUserAddressRepository repository, Guid userId, Guid keepAddressId, CancellationToken cancellationToken) =>
        ClearPreviousHolderAsync(repository, a => a.UserId == userId && a.IsShippingAddress, a => a.IsShippingAddress = false, keepAddressId, cancellationToken);

    public static Task ClearPreviousBillingHolderAsync(IUserAddressRepository repository, Guid userId, Guid keepAddressId, CancellationToken cancellationToken) =>
        ClearPreviousHolderAsync(repository, a => a.UserId == userId && a.IsBillingAddress, a => a.IsBillingAddress = false, keepAddressId, cancellationToken);

    private static async Task ClearPreviousHolderAsync(IUserAddressRepository repository, Expression<Func<UserAddress, bool>> predicate, Action<UserAddress> clearFlag, Guid keepAddressId, CancellationToken cancellationToken)
    {
        var oldHolder = await repository.GetAsync(predicate, cancellationToken: cancellationToken);
        if (oldHolder is null || oldHolder.Id == keepAddressId)
        {
            return;
        }

        clearFlag(oldHolder);
        _ = await repository.UpdateAsync(oldHolder);
    }
}
