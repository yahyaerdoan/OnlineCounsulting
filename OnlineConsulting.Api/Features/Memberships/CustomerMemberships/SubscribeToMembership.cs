using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.SubscribeToMembership;
using OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.Constants;
using OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.GetMyAccountCredit;
using OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.SpendAccountCredit;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

public class SubscribeToMembership : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/memberships/subscribe", Handle)
            .WithTags("Memberships/CustomerMemberships")
            .RequireAuthorization()
            .WithName("SubscribeToMembership")
            .WithDescription("Subscribes the current user to a membership plan using an already-tokenized payment method id (e.g. from Stripe.js). CreditToApplyAmount, if given, is clamped to the user's referral-reward credit balance here and to the plan price by the handler, then applied as a one-time discount.");
    }

    private static async Task<IResult> Handle([FromBody] SubscribeToMembershipCommand command, ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
            return currentUser.ToEnvelopedResult(httpContext);

        var requestedCreditAmount = 0m;
        if (command.CreditToApplyAmount is > 0)
        {
            var creditSummary = await sender.Send(new GetMyAccountCreditQuery(currentUser.Data.Id));
            if (!creditSummary.IsSuccessful || creditSummary.Data is null)
                return creditSummary.ToEnvelopedResult(httpContext);

            requestedCreditAmount = Math.Min(command.CreditToApplyAmount.Value, creditSummary.Data.Balance);
        }

        var result = await sender.Send(command with
        {
            UserId = currentUser.Data.Id,
            Email = currentUser.Data.Email,
            CreditToApplyAmount = requestedCreditAmount > 0 ? requestedCreditAmount : null,
        });

        if (result.IsSuccessful && result.Data is not null && result.Data.AppliedCreditAmount is > 0)
        {
            await sender.Send(new SpendAccountCreditCommand(
                currentUser.Data.Id,
                result.Data.AppliedCreditAmount.Value,
                "Applied to membership subscription",
                AccountCreditSourceTypes.MembershipDiscount,
                result.Data.CustomerMembershipId));
        }

        return result.ToEnvelopedResult(httpContext);
    }
}
