using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.GetMyEquipment;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Equipment;

public class GetMyEquipment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/equipment/mine", Handle)
            .WithTags("Equipment")
            .RequireAuthorization()
            .WithName("GetMyEquipment")
            .WithDescription("Returns the current user's installed equipment - the customer portal's equipment health panel.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
            return currentUser.ToEnvelopedResult(httpContext);

        var result = await sender.Send(new GetMyEquipmentQuery(currentUser.Data.Id));
        return result.ToEnvelopedResult(httpContext);
    }
}
