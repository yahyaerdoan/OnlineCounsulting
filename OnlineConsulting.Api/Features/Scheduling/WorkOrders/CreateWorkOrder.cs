using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.CreateEquipmentItem;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.DeleteEquipmentItem;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.CreateWorkOrder;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Scheduling.WorkOrders;

/// <summary>
/// Wraps CreateWorkOrderCommand with an optional NewEquipment sub-request so one call can record both; orchestration lives here to keep Equipment and
/// Scheduling modules decoupled. The two modules commit separately, so equipment created for a work order that then fails is deleted again.
/// </summary>
public record CreateWorkOrderRequest(Guid AppointmentId, Guid TechnicianUserId, string? PartsUsed, string? TechnicianNotes, DateTimeOffset? CompletedAt, Guid? EquipmentId, NewEquipmentRequest? NewEquipment,
    List<WorkOrderChargeInput>? Charges = null)
{
    public CreateWorkOrderCommand ToCommand(Guid? equipmentId) => new(AppointmentId, TechnicianUserId, PartsUsed, TechnicianNotes, CompletedAt, equipmentId, Charges);
}

public record NewEquipmentRequest(Guid CustomerUserId, string Type, string? Brand, string? Model, string? SerialNumber, DateTimeOffset? InstallDate, DateTimeOffset? WarrantyExpiresAt, string? Notes)
{
    public CreateEquipmentItemCommand ToCommand() => new(CustomerUserId, Type, Brand, Model, SerialNumber, InstallDate, WarrantyExpiresAt, Notes);
}

public class CreateWorkOrder : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/work-orders", Handle)
            .WithTags("Scheduling/WorkOrders")
            .RequireAuthorization()
            .WithName("CreateWorkOrder")
            .WithDescription("Records completed work against an appointment (parts used, technician notes) - this is what marks the appointment Completed. Pass NewEquipment instead of EquipmentId to record a newly-installed piece of equipment in the same call.");
    }

    private static async Task<IResult> Handle([FromBody] CreateWorkOrderRequest request, ISender sender, ILogger<CreateWorkOrder> logger, HttpContext httpContext)
    {
        if (request.EquipmentId is not null || request.NewEquipment is null)
        {
            return (await sender.Send(request.ToCommand(request.EquipmentId))).ToEnvelopedResult(httpContext);
        }

        var createEquipmentResult = await sender.Send(request.NewEquipment.ToCommand());

        if (!createEquipmentResult.IsSuccessful)
        {
            return createEquipmentResult.ToEnvelopedResult(httpContext);
        }

        var equipmentId = createEquipmentResult.Data;

        var succeeded = false;
        try
        {
            var result = await sender.Send(request.ToCommand(equipmentId));
            succeeded = result.IsSuccessful;
            return result.ToEnvelopedResult(httpContext);
        }
        finally
        {
            if (!succeeded)
            {
                await DiscardEquipmentAsync(sender, logger, equipmentId);
            }
        }
    }

    private static async Task DiscardEquipmentAsync(ISender sender, ILogger logger, Guid equipmentId)
    {
        try
        {
            var deleteResult = await sender.Send(new DeleteEquipmentItemCommand(equipmentId), CancellationToken.None);

            if (!deleteResult.IsSuccessful)
            {
                logger.LogError("Equipment item {EquipmentId} was created for a work order that failed and could not be deleted: {Status} {Detail}", equipmentId, deleteResult.Status, deleteResult.Detail ?? deleteResult.Title);
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Equipment item {EquipmentId} was created for a work order that failed and could not be deleted.", equipmentId);
        }
    }
}
