using MediatR;
using OnlineConsulting.SharedKernel.CurrentUser;
using OnlineConsulting.SharedKernel.GuestIdentity;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Baskets;

/// <summary>Resolves whether the caller is an authenticated user (UserId via JWT) or an anonymous guest (GuestId via cookie) exactly once, instead of every basket endpoint re-deriving it.</summary>
internal static class BasketOwnerResolver
{
    public static (Guid? UserId, Guid? GuestId) Resolve(ICurrentUserAccessor currentUser, IGuestIdAccessor guestIdAccessor) =>
        currentUser.Id is { } userId ? (userId, null) : (null, guestIdAccessor.GetOrCreateGuestId());
}
