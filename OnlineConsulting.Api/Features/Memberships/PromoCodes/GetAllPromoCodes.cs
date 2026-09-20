using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.GetAllPromoCodes;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.PromoCodes;

public class GetAllPromoCodes : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/promo-codes", Handle)
            .WithTags("Memberships/PromoCodes")
            .RequireAuthorization()
            .WithName("GetAllPromoCodes")
            .WithDescription("Returns the current tenant's promo codes, paginated (admin).");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetAllPromoCodesQuery(PageRequestFactory.Create(index, size)));
        return result.ToEnvelopedResult(httpContext);
    }
}
