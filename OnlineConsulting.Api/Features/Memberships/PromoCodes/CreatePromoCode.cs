using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.CreatePromoCode;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.PromoCodes;

public class CreatePromoCode : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/promo-codes", Handle)
            .WithTags("Memberships/PromoCodes")
            .RequireAuthorization()
            .WithName("CreatePromoCode")
            .WithDescription("Creates a promo/discount code (admin).");
    }

    private static async Task<IResult> Handle([FromBody] CreatePromoCodeCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
