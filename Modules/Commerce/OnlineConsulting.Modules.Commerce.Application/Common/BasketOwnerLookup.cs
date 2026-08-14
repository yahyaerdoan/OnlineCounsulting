using System.Linq.Expressions;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Common;

// A basket belongs to exactly one owner - a logged-in user (UserId) or an anonymous guest
// (GuestId), never both (see Basket.cs). Every Basket handler needs to look a basket up by
// whichever one the caller actually has, so the branching lives here once instead of five times.
public static class BasketOwnerLookup
{
    public static Expression<Func<Basket, bool>> Predicate(Guid? userId, Guid? guestId) => userId is { } uid ? b => b.UserId == uid : b => b.GuestId == guestId;
}
