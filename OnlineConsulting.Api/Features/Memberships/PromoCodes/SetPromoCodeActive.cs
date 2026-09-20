using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.SetPromoCodeActive;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.PromoCodes;

public class SetPromoCodeActive : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/promo-codes/{id:guid}/active", Handle)
            .WithTags("Memberships/PromoCodes")
            .RequireAuthorization()
            .WithName("SetPromoCodeActive")
            .WithDescription("Activates or deactivates a promo code (admin).");
    }

    private static async Task<IResult> Handle(Guid id, bool isActive, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new SetPromoCodeActiveCommand(id, isActive));
        return result.ToEnvelopedResult(httpContext);
    }
}
