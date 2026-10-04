using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Configurations.Extensions;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.RedeemReferralCode;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Referrals;

public class RedeemReferralCode : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/referrals/redeem", Handle)
            .WithTags("Referrals")
            .RequireAuthorization()
            .RequireRateLimiting(ServiceRegistration.ReferralRedeemRateLimiterPolicy)
            .WithName("RedeemReferralCode")
            .WithDescription("Redeems a referral code on behalf of the current user - at most once per user.");
    }

    private static async Task<IResult> Handle([FromBody] RedeemReferralCodeCommand command, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(command with { ReferredUserId = user.Id })))
            .ToEnvelopedResult(httpContext);
}
