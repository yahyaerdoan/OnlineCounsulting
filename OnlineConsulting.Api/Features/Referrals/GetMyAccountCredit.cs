using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.GetMyAccountCredit;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

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
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new GetMyAccountCreditQuery(user.Id))))
            .ToEnvelopedResult(httpContext);
}
