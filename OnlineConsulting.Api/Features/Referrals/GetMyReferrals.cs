using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.GetMyReferrals;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Referrals;

public class GetMyReferrals : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/referrals/mine", Handle)
            .WithTags("Referrals")
            .RequireAuthorization()
            .WithName("GetMyReferrals")
            .WithDescription("Returns the referrals the current user has made as a referrer.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new GetMyReferralsQuery(user.Id))))
            .ToEnvelopedResult(httpContext);
}
