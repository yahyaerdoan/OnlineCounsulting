using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Referrals.Application.Features.ReferralCodes.GetOrCreateReferralCode;
using OnlineConsulting.SharedKernel.CurrentUser;
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
            .WithDescription("Returns the current user's referral code, creating one on first call.")
            .ProducesEnveloped<string>()
            .ProducesEnveloped<string>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext) =>
        (await sender.Send(new GetOrCreateReferralCodeCommand(currentUser.RequiredId()))).ToEnvelopedResult(httpContext);
}
