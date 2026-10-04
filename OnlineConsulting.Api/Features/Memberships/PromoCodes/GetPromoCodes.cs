using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.GetPromoCodes;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.PromoCodes;

public class GetPromoCodes : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/promo-codes", Handle)
            .WithTags("Memberships/PromoCodes")
            .RequireAuthorization()
            .WithName("GetPromoCodes")
            .WithDescription("Returns the current tenant's promo codes, paginated (admin).");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetPromoCodesQuery(PageRequestFactory.Create(index, size)));
        return result.ToEnvelopedResult(httpContext);
    }
}
