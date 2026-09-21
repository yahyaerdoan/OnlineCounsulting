using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.CreateEquipmentItem;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.CreateWorkOrder;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Scheduling.WorkOrders;

/// <summary>Wraps CreateWorkOrderCommand with an optional NewEquipment sub-request so one call can record both; orchestration lives here to keep Equipment and Scheduling modules decoupled.</summary>
public record CreateWorkOrderRequest(Guid AppointmentId, Guid TechnicianUserId, string? PartsUsed, string? TechnicianNotes, DateTimeOffset? CompletedAt, Guid? EquipmentId, NewEquipmentRequest? NewEquipment);

public record NewEquipmentRequest(Guid CustomerUserId, string Type, string? Brand, string? Model, string? SerialNumber, DateTimeOffset? InstallDate, DateTimeOffset? WarrantyExpiresAt, string? Notes);

public class CreateWorkOrder : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/work-orders", Handle)
            .WithTags("Scheduling/WorkOrders")
            .RequireAuthorization()
            .WithName("CreateWorkOrder")
            .WithDescription("Records completed work against an appointment (parts used, technician notes) - this is what marks the appointment Completed. Pass NewEquipment instead of EquipmentId to record a newly-installed piece of equipment in the same call.");
    }

    private static async Task<IResult> Handle([FromBody] CreateWorkOrderRequest request, ISender sender, HttpContext httpContext)
    {
        var equipmentId = request.EquipmentId;

        if (equipmentId is null && request.NewEquipment is not null)
        {
            var newEquipment = request.NewEquipment;

            var createEquipmentResult = await sender
                .Send(new CreateEquipmentItemCommand(newEquipment.CustomerUserId, newEquipment.Type, newEquipment.Brand, newEquipment.Model, newEquipment.SerialNumber, newEquipment.InstallDate, newEquipment.WarrantyExpiresAt, newEquipment.Notes));

            if (!createEquipmentResult.IsSuccessful)
            {
                return createEquipmentResult.ToEnvelopedResult(httpContext);
            }

            equipmentId = createEquipmentResult.Data;
        }

        var command = new CreateWorkOrderCommand(request.AppointmentId, request.TechnicianUserId, request.PartsUsed, request.TechnicianNotes, request.CompletedAt, equipmentId);

        var result = await sender.Send(command);

        return result.ToEnvelopedResult(httpContext);
    }
}
