using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.CreateEquipmentItem;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Equipment;

public class CreateEquipmentItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/equipment", Handle)
            .WithTags("Equipment")
            .RequireAuthorization()
            .WithName("CreateEquipmentItem")
            .WithDescription("Records a piece of a customer's installed equipment (admin/technician).")
            .ProducesEnveloped<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] CreateEquipmentItemRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateEquipmentItemRequest(Guid UserId, string Type, string? Brand, string? Model, string? SerialNumber, DateTimeOffset? InstallDate, DateTimeOffset? WarrantyExpiresAt, string? Notes)
{
    public CreateEquipmentItemCommand ToCommand() => new(UserId, Type, Brand, Model, SerialNumber, InstallDate, WarrantyExpiresAt, Notes);
}
