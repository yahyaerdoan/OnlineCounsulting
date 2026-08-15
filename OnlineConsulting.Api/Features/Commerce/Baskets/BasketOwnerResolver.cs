using MediatR;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.SharedKernel.GuestIdentity;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Baskets;

/// <summary>Resolves whether the caller is an authenticated user (UserId via JWT) or an anonymous guest (GuestId via cookie) exactly once, instead of every basket endpoint re-deriving it.</summary>
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
