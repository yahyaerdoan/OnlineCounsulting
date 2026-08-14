using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.MergeGuestBasket;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Login;
using OnlineConsulting.SharedKernel.GuestIdentity;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Auth;

public class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", Handle)
            .WithTags("Identity/Auth")
            .WithName("Login")
            .WithDescription("Validates credentials and issues a JWT access token + refresh token.");
    }

    private static async Task<IResult> Handle([FromBody] LoginCommand command, ISender sender, HttpContext httpContext, IGuestIdAccessor guestIdAccessor)
    {
        var result = await sender.Send(command);
        if (!result.IsSuccessful || result.Data is null)
            return result.ToEnvelopedResult(httpContext);

        // If this browser shopped as a guest before logging in, fold that basket into the user's
        // own basket - the guest cookie is only meaningful while unauthenticated.
        if (guestIdAccessor.TryGetGuestId() is { } guestId)
        {
            await sender.Send(new MergeGuestBasketCommand(result.Data.UserId, guestId));
            guestIdAccessor.ClearGuestId();
        }

        return result.ToEnvelopedResult(httpContext);
    }
}
