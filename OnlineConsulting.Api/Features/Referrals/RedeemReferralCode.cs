using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Configurations.Extensions;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.RedeemReferralCode;
using OnlineConsulting.SharedKernel.CurrentUser;
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
            .WithDescription("Redeems a referral code on behalf of the current user - at most once per user.")
            .ProducesEnveloped<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, [FromBody] RedeemReferralCodeRequest request, ISender sender, HttpContext httpContext)
        => (await sender.Send(request.ToCommand(currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}

public record RedeemReferralCodeRequest(string Code)
{
    public RedeemReferralCodeCommand ToCommand(Guid referredUserId) => new(referredUserId, Code);
}
