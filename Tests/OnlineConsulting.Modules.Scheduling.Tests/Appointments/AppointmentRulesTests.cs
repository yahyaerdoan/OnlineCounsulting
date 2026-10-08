using OnlineConsulting.Modules.Scheduling.Domain;

namespace OnlineConsulting.Modules.Scheduling.Tests.Appointments;

public class AppointmentRulesTests
{
    [Theory]
    [InlineData(AppointmentStatuses.Pending, false)]
    [InlineData(AppointmentStatuses.Confirmed, false)]
    [InlineData(AppointmentStatuses.PendingPayment, false)]
    [InlineData(AppointmentStatuses.Cancelled, true)]
    [InlineData(AppointmentStatuses.Completed, true)]
    public void IsClosed(string status, bool expected) =>
        Assert.Equal(expected, AppointmentRules.IsClosed(status));

    [Theory]
    [InlineData(AppointmentStatuses.Pending, true)]
    [InlineData(AppointmentStatuses.Confirmed, false)]
    [InlineData(AppointmentStatuses.Cancelled, false)]
    [InlineData(AppointmentStatuses.Completed, false)]
    public void CanBeConfirmed(string status, bool expected) =>
        Assert.Equal(expected, AppointmentRules.CanBeConfirmed(status));

    [Theory]
    [InlineData(AppointmentStatuses.Pending, true)]
    [InlineData(AppointmentStatuses.Confirmed, true)]
    [InlineData(AppointmentStatuses.PendingPayment, false)]
    [InlineData(AppointmentStatuses.Cancelled, false)]
    [InlineData(AppointmentStatuses.Completed, false)]
    public void CanBeCancelled(string status, bool expected) =>
        Assert.Equal(expected, AppointmentRules.CanBeCancelled(status));
}
