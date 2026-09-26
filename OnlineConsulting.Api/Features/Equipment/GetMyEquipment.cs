using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.GetMyEquipment;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Equipment;

public class GetMyEquipment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/equipment/mine", Handle)
            .WithTags("Equipment")
            .RequireAuthorization()
            .WithName("GetMyEquipment")
            .WithDescription("Returns the current user's installed equipment - the customer portal's equipment health panel.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new GetMyEquipmentQuery(user.Id))))
            .ToEnvelopedResult(httpContext);
}
