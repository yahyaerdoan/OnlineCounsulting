using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.GetMyReferrals;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Referrals;

public class GetMyReferrals : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/referrals/mine", Handle)
            .WithTags("Referrals")
            .RequireAuthorization()
            .WithName("GetMyReferrals")
            .WithDescription("Returns the referrals the current user has made as a referrer.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
        {
            return currentUser.ToEnvelopedResult(httpContext);
        }

        var result = await sender.Send(new GetMyReferralsQuery(currentUser.Data.Id));
        return result.ToEnvelopedResult(httpContext);
    }
}
