using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.SubscribeToMembership;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

public class SubscribeToMembership : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/memberships/subscribe", Handle)
            .WithTags("Memberships/CustomerMemberships")
            .RequireAuthorization()
            .WithName("SubscribeToMembership")
            .WithDescription("Subscribes the current user to a membership plan using an already-tokenized payment method id (e.g. from Stripe.js). CreditToApplyAmount, if given, is clamped to the user's referral-reward credit balance and to what is left of the plan price after any promo code, then applied as a one-time discount. The credit is reserved before the card is charged and returned if payment setup fails; retrying the same attempt never debits it twice.");
    }

    private static async Task<IResult> Handle([FromBody] SubscribeToMembershipCommand command, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(command with { UserId = user.Id, Email = user.Email })))
            .ToEnvelopedResult(httpContext);
}
