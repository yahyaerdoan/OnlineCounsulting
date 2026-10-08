using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Contracts;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.ValidatePromoCode;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

public class ValidatePromoCode : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/memberships/promo-codes/validate", Handle)
            .WithTags("Memberships/CustomerMemberships")
            .RequireAuthorization()
            .WithName("ValidatePromoCode")
            .WithDescription("Previews a promo code's discount for the current user against a membership plan - does not redeem it.")
            .ProducesEnveloped<ValidatePromoCodeResult>();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, [FromBody] ValidatePromoCodeRequest request, ISender sender, HttpContext httpContext)
        => (await sender.Send(new ValidatePromoCodeCommand(currentUser.RequiredId(), request.Code, request.MembershipPlanId)))
            .ToEnvelopedResult(httpContext);
}

public record ValidatePromoCodeRequest(string Code, Guid MembershipPlanId);
