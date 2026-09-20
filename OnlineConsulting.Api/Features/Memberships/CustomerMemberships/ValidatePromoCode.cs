using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.ValidatePromoCode;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

public class ValidatePromoCode : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/memberships/promo-codes/validate", Handle)
            .WithTags("Memberships/CustomerMemberships")
            .RequireAuthorization()
            .WithName("ValidatePromoCode")
            .WithDescription("Previews a promo code's discount for the current user against a membership plan - does not redeem it.");
    }

    private static async Task<IResult> Handle([FromBody] ValidatePromoCodeRequest request, ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());

        if (!currentUser.IsSuccessful || currentUser.Data is null)
        {
            return currentUser.ToEnvelopedResult(httpContext);
        }

        var result = await sender.Send(new ValidatePromoCodeCommand(currentUser.Data.Id, request.Code, request.MembershipPlanId));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record ValidatePromoCodeRequest(string Code, Guid MembershipPlanId);
