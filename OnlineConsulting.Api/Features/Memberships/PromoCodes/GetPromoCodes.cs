using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Contracts;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.GetPromoCodes;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.PromoCodes;

public class GetPromoCodes : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/promo-codes", Handle)
            .WithTags("Memberships/PromoCodes")
            .RequireAuthorization()
            .WithName("GetPromoCodes")
            .WithDescription("Returns the current tenant's promo codes, paginated (admin).")
            .ProducesEnveloped<Paginate<PromoCodeResponse>>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetPromoCodesQuery(PageRequestFactory.Create(index, size)));
        return result.ToEnvelopedResult(httpContext);
    }
}
