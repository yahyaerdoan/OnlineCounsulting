using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.CompleteReferral;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Referrals;

public class CompleteReferral : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/referrals/{id:guid}/complete", Handle)
            .WithTags("Referrals")
            .RequireAuthorization()
            .WithName("CompleteReferral")
            .WithDescription("Marks a referral as rewarded (admin).");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] CompleteReferralCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
