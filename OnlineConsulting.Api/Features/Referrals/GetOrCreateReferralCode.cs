using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Referrals.Application.Features.ReferralCodes.GetOrCreateReferralCode;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Referrals;

public class GetOrCreateReferralCode : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/referrals/my-code", Handle)
            .WithTags("Referrals")
            .RequireAuthorization()
            .WithName("GetOrCreateReferralCode")
            .WithDescription("Returns the current user's referral code, creating one on first call.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
            return currentUser.ToEnvelopedResult(httpContext);

        var result = await sender.Send(new GetOrCreateReferralCodeCommand(currentUser.Data.Id));
        return result.ToEnvelopedResult(httpContext);
    }
}
