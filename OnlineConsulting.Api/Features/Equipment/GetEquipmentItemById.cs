using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Contracts;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.GetEquipmentItemById;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Equipment;

public class GetEquipmentItemById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/equipment/{id:guid}", Handle)
            .WithTags("Equipment")
            .RequireAuthorization()
            .WithName("GetEquipmentItemById")
            .WithDescription("Returns one piece of a customer's installed equipment (admin/technician).")
            .ProducesEnveloped<EquipmentItemResponse>();
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetEquipmentItemByIdQuery(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
