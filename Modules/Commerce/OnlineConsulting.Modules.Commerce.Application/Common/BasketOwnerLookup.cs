using OnlineConsulting.Modules.Commerce.Domain;
using System.Linq.Expressions;

namespace OnlineConsulting.Modules.Commerce.Application.Common;

/// <summary>Looks up a basket by whichever single owner (UserId xor GuestId) the caller has, shared by every Basket handler.</summary>
public static class BasketOwnerLookup
{
    public static Expression<Func<Basket, bool>> Predicate(Guid? userId, Guid? guestId) => userId is { } uid ? b => b.UserId == uid : b => b.GuestId == guestId;
}
