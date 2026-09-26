using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.ValidatePromoCode;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

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
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new ValidatePromoCodeCommand(user.Id, request.Code, request.MembershipPlanId))))
            .ToEnvelopedResult(httpContext);
}

public record ValidatePromoCodeRequest(string Code, Guid MembershipPlanId);
