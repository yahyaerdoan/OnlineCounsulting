using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.DeleteEquipmentItem;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Equipment;

public class DeleteEquipmentItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/equipment/{id:guid}", Handle)
            .WithTags("Equipment")
            .RequireAuthorization()
            .WithName("DeleteEquipmentItem")
            .WithDescription("Deletes a piece of a customer's installed equipment (admin/technician).");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteEquipmentItemCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
