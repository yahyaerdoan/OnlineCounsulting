using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.Contracts;
using OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.GetMyAccountCredit;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Referrals;

public class GetMyAccountCredit : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/referrals/my-credit", Handle)
            .WithTags("Referrals")
            .RequireAuthorization()
            .WithName("GetMyAccountCredit")
            .WithDescription("Returns the current user's referral-reward account credit balance and ledger.")
            .ProducesEnveloped<AccountCreditSummaryResponse>();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetMyAccountCreditQuery(currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
