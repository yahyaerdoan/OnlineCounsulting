namespace OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.CreateWorkOrder;

/// <summary>One billable charge the technician records for the visit (the service itself, parts, extra labor).</summary>
public record WorkOrderChargeInput(string Description, decimal Quantity, decimal UnitPrice, int TaxRate);
