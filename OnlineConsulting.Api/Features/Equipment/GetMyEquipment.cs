using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Contracts;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.GetMyEquipment;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Equipment;

public class GetMyEquipment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/equipment/mine", Handle)
            .WithTags("Equipment")
            .RequireAuthorization()
            .WithName("GetMyEquipment")
            .WithDescription("Returns the current user's installed equipment - the customer portal's equipment health panel.")
            .ProducesEnveloped<List<EquipmentItemResponse>>();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetMyEquipmentQuery(currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
