using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.UpdateEquipmentItem;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Equipment;

public class UpdateEquipmentItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/equipment/{id:guid}", Handle)
            .WithTags("Equipment")
            .RequireAuthorization()
            .WithName("UpdateEquipmentItem")
            .WithDescription("Updates a piece of a customer's installed equipment (admin/technician).")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateEquipmentItemRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateEquipmentItemRequest(string Type, string? Brand, string? Model, string? SerialNumber, DateTimeOffset? InstallDate, DateTimeOffset? WarrantyExpiresAt, string? Notes)
{
    public UpdateEquipmentItemCommand ToCommand(Guid id) => new(id, Type, Brand, Model, SerialNumber, InstallDate, WarrantyExpiresAt, Notes);
}
