using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Referrals.Application.Features.ReferralCodes.GetOrCreateReferralCode;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Referrals;

public class GetOrCreateReferralCode : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/referrals/my-code", Handle)
            .WithTags("Referrals")
            .RequireAuthorization()
            .WithName("GetOrCreateReferralCode")
            .WithDescription("Returns the current user's referral code, creating one on first call.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext) =>
        (await sender.Send(new GetCurrentUserQuery()).BindAsync(user => sender.Send(new GetOrCreateReferralCodeCommand(user.Id)))).ToEnvelopedResult(httpContext);
}
