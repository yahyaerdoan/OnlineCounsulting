using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Contracts;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.SubscribeToMembership;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

public class SubscribeToMembership : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/memberships/subscribe", Handle)
            .WithTags("Memberships/CustomerMemberships")
            .RequireAuthorization()
            .WithName("SubscribeToMembership")
            .WithDescription("Subscribes the current user to a membership plan using an already-tokenized payment method id (e.g. from Stripe.js). CreditToApplyAmount, if given, is clamped to the user's referral-reward credit balance and to what is left of the plan price after any promo code, then applied as a one-time discount. The credit is reserved before the card is charged and returned if payment setup fails; retrying the same attempt never debits it twice.")
            .ProducesEnveloped<SubscribeToMembershipResult>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, [FromBody] SubscribeToMembershipRequest request, ISender sender, HttpContext httpContext)
        => (await sender.Send(request.ToCommand(currentUser.RequiredId(), currentUser.RequiredEmail())))
            .ToEnvelopedResult(httpContext);
}

public record SubscribeToMembershipRequest(Guid MembershipPlanId, string PaymentMethodId, decimal? CreditToApplyAmount = null, string? PromoCode = null)
{
    public SubscribeToMembershipCommand ToCommand(Guid userId, string email) => new(userId, email, MembershipPlanId, PaymentMethodId, CreditToApplyAmount, PromoCode);
}
