using MediatR;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.SharedKernel.GuestIdentity;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Baskets;

// Basket endpoints work for both logged-in users and anonymous guests - resolves which one the
// caller is exactly once instead of every endpoint re-deriving it. Authenticated -> UserId via the
// current JWT; otherwise -> GuestId from the guest_id cookie (issued here if the caller doesn't
// have one yet).
internal static class BasketOwnerResolver
{
    public static async Task<(Guid? UserId, Guid? GuestId, IResult? Error)> ResolveAsync(ISender sender, HttpContext httpContext, IGuestIdAccessor guestIdAccessor)
    {
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            var currentUser = await sender.Send(new GetCurrentUserQuery());
            if (!currentUser.IsSuccessful || currentUser.Data is null)
                return (null, null, currentUser.ToEnvelopedResult(httpContext));

            return (currentUser.Data.Id, null, null);
        }

        return (null, guestIdAccessor.GetOrCreateGuestId(), null);
    }
}
