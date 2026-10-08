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
        _ = app.MapPost("/promo-codes", Handle)
            .WithTags("Memberships/PromoCodes")
            .RequireAuthorization()
            .WithName("CreatePromoCode")
            .WithDescription("Creates a promo/discount code (admin).")
            .ProducesEnveloped<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] CreatePromoCodeRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreatePromoCodeRequest(string Code, string DiscountType, decimal DiscountValue, int? MaxRedemptions, DateTimeOffset? ExpiresAt, Guid? MembershipPlanId)
{
    public CreatePromoCodeCommand ToCommand() => new(Code, DiscountType, DiscountValue, MaxRedemptions, ExpiresAt, MembershipPlanId);
}
