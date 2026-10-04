using OnlineConsulting.Modules.Scheduling.Domain;

namespace OnlineConsulting.Modules.Scheduling.Tests.WorkOrders;

public class WorkOrderTests
{
    private static readonly DateTimeOffset Start = new(2026, 10, 6, 15, 0, 0, TimeSpan.Zero);

    private static Appointment Visit() =>
        Appointment.Request(Guid.NewGuid(), Guid.NewGuid(), Start, Start.AddHours(2), AppointmentMeetingTypes.InPerson, null, "1 Main St", null);

    [Fact]
    public void RecordFor_CompletesTheVisitAndKeepsTheDetails()
    {
        var appointment = Visit();
        var technician = Guid.NewGuid();
        var equipment = Guid.NewGuid();

        var workOrder = WorkOrder.RecordFor(appointment, technician, "  Filter 16x25  ", " ", Start.AddHours(2), equipment);

        Assert.Equal(AppointmentStatuses.Completed, appointment.Status);
        Assert.Equal(appointment.Id, workOrder.AppointmentId);
        Assert.Equal(technician, workOrder.TechnicianUserId);
        Assert.Equal("Filter 16x25", workOrder.PartsUsed);
        Assert.Null(workOrder.TechnicianNotes);
        Assert.Equal(Start.AddHours(2), workOrder.CompletedAt);
        Assert.Equal(equipment, workOrder.EquipmentId);
    }

    [Fact]
    public void RecordFor_WithoutTechnician_ThrowsAndLeavesTheVisitOpen()
    {
        var appointment = Visit();

        _ = Assert.Throws<ArgumentException>(() => WorkOrder.RecordFor(appointment, Guid.Empty, null, null, Start, null));
        Assert.Equal(AppointmentStatuses.Pending, appointment.Status);
    }

    [Fact]
    public void RecordFor_ClosedVisit_Throws()
    {
        var appointment = Visit();
        appointment.Cancel();

        _ = Assert.Throws<InvalidOperationException>(() => WorkOrder.RecordFor(appointment, Guid.NewGuid(), null, null, Start, null));
    }
}
