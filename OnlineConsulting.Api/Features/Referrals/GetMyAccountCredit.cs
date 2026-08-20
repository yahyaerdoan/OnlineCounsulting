using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.GetMyAccountCredit;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Referrals;

public class GetMyAccountCredit : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/referrals/my-credit", Handle)
            .WithTags("Referrals")
            .RequireAuthorization()
            .WithName("GetMyAccountCredit")
            .WithDescription("Returns the current user's referral-reward account credit balance and ledger.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
        {
            return currentUser.ToEnvelopedResult(httpContext);
        }

        var result = await sender.Send(new GetMyAccountCreditQuery(currentUser.Data.Id));
        return result.ToEnvelopedResult(httpContext);
    }
}
