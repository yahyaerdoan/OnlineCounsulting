using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.GetAllReferrals;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Referrals;

public class GetAllReferrals : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/referrals", Handle)
            .WithTags("Referrals")
            .RequireAuthorization()
            .WithName("GetAllReferrals")
            .WithDescription("Returns all referrals, paginated (admin).");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetAllReferralsQuery(PageRequestFactory.Create(index, size)));
        return result.ToEnvelopedResult(httpContext);
    }
}
