using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Scheduling.Domain;
using OnlineConsulting.SharedKernel.LiveUpdates;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.LiveUpdates;

/// <summary>Appointments notify both the customer and the assigned technician (old and new on reassignment); work orders and
/// their photos also refresh the customer's equipment history, reached through the appointment.</summary>
public static class SchedulingUserDataChangeRules
{
    public static void Configure(UserDataChangeRuleSet rules) => rules
        .ForUserProperties<Appointment>(UserDataTopics.Appointments, nameof(Appointment.UserId), nameof(Appointment.AssignedTechnicianUserId))
        .For<AppointmentMediaItem>(UserDataTopics.Appointments, async (entry, cancellationToken) =>
            await AppointmentCustomerAsync(entry.Context, entry.Entity.AppointmentId, cancellationToken) is Guid userId ? [userId] : [])
        .For<WorkOrder>(UserDataTopics.Appointments, async (entry, cancellationToken) => await WorkOrderUsersAsync(entry.Context, entry.Entity, cancellationToken))
        .For<WorkOrder>(UserDataTopics.Equipment, async (entry, cancellationToken) =>
            await AppointmentCustomerAsync(entry.Context, entry.Entity.AppointmentId, cancellationToken) is Guid userId ? [userId] : [])
        .For<WorkOrderMediaItem>(UserDataTopics.Equipment, async (entry, cancellationToken) =>
            await WorkOrderCustomerAsync(entry.Context, entry.Entity.WorkOrderId, cancellationToken) is Guid userId ? [userId] : []);

    private static async Task<IEnumerable<Guid>> WorkOrderUsersAsync(DbContext context, WorkOrder workOrder, CancellationToken cancellationToken) =>
        await AppointmentCustomerAsync(context, workOrder.AppointmentId, cancellationToken) is Guid customerId
            ? [customerId, workOrder.TechnicianUserId]
            : [workOrder.TechnicianUserId];

    private static async Task<Guid?> AppointmentCustomerAsync(DbContext context, Guid appointmentId, CancellationToken cancellationToken) =>
        context.Set<Appointment>().Local.FirstOrDefault(a => a.Id == appointmentId) is { } tracked
            ? tracked.UserId
            : await context.Set<Appointment>().AsNoTracking().Where(a => a.Id == appointmentId).Select(a => (Guid?)a.UserId).FirstOrDefaultAsync(cancellationToken);

    private static async Task<Guid?> WorkOrderCustomerAsync(DbContext context, Guid workOrderId, CancellationToken cancellationToken)
    {
        var appointmentId = context.Set<WorkOrder>().Local.FirstOrDefault(w => w.Id == workOrderId) is { } tracked
            ? tracked.AppointmentId
            : await context.Set<WorkOrder>().AsNoTracking().Where(w => w.Id == workOrderId).Select(w => (Guid?)w.AppointmentId).FirstOrDefaultAsync(cancellationToken);

        return appointmentId is Guid id ? await AppointmentCustomerAsync(context, id, cancellationToken) : null;
    }
}
