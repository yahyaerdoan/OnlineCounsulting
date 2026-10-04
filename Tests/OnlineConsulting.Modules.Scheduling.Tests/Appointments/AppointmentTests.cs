using OnlineConsulting.Modules.Scheduling.Domain;

namespace OnlineConsulting.Modules.Scheduling.Tests.Appointments;

public class AppointmentTests
{
    private static readonly DateTimeOffset Start = new(2026, 10, 10, 9, 0, 0, TimeSpan.Zero);

    private static Appointment RequestInPerson() =>
        Appointment.Request(Guid.NewGuid(), Guid.NewGuid(), Start, Start.AddHours(2), AppointmentMeetingTypes.InPerson, null, "1 Main St", "Gate code 42");

    private static Appointment InStatus(string status)
    {
        var appointment = RequestInPerson();
        switch (status)
        {
            case AppointmentStatuses.Confirmed:
                appointment.Confirm();
                break;
            case AppointmentStatuses.Cancelled:
                appointment.Cancel();
                break;
            case AppointmentStatuses.Completed:
                appointment.Complete();
                break;
        }

        return appointment;
    }

    [Fact]
    public void Request_InPerson_IsPendingWithTrimmedAddressAndNoTopic()
    {
        var appointment = Appointment.Request(Guid.NewGuid(), null, Start, Start.AddHours(1), AppointmentMeetingTypes.InPerson, "Ignored", "  1 Main St  ", null);

        Assert.Equal(AppointmentStatuses.Pending, appointment.Status);
        Assert.Equal("1 Main St", appointment.ServiceAddress);
        Assert.Null(appointment.Topic);
        Assert.False(appointment.RequiresPrepayment);
        Assert.Null(appointment.AssignedTechnicianUserId);
    }

    [Fact]
    public void Request_Online_KeepsTrimmedTopicAndNoAddress()
    {
        var appointment = Appointment.Request(Guid.NewGuid(), null, Start, Start.AddHours(1), AppointmentMeetingTypes.Online, "  New system quote  ", "1 Main St", null);

        Assert.Equal("New system quote", appointment.Topic);
        Assert.Null(appointment.ServiceAddress);
    }

    [Fact]
    public void Request_EndingBeforeItStarts_Throws() =>
        Assert.Throws<ArgumentException>(() =>
            Appointment.Request(Guid.NewGuid(), null, Start, Start, AppointmentMeetingTypes.InPerson, null, "1 Main St", null));

    [Fact]
    public void Request_WithUnknownMeetingType_Throws() =>
        Assert.Throws<ArgumentException>(() =>
            Appointment.Request(Guid.NewGuid(), null, Start, Start.AddHours(1), "Phone", null, "1 Main St", null));

    [Fact]
    public void Request_OnlineWithoutTopic_Throws() =>
        Assert.ThrowsAny<ArgumentException>(() =>
            Appointment.Request(Guid.NewGuid(), null, Start, Start.AddHours(1), AppointmentMeetingTypes.Online, " ", null, null));

    [Fact]
    public void Request_InPersonWithoutAddress_Throws() =>
        Assert.ThrowsAny<ArgumentException>(() =>
            Appointment.Request(Guid.NewGuid(), null, Start, Start.AddHours(1), AppointmentMeetingTypes.InPerson, null, null, null));

    [Fact]
    public void Confirm_WhenPending_Confirms()
    {
        var appointment = RequestInPerson();

        appointment.Confirm();

        Assert.Equal(AppointmentStatuses.Confirmed, appointment.Status);
        Assert.False(appointment.CanBeConfirmed);
    }

    [Theory]
    [InlineData(AppointmentStatuses.Confirmed)]
    [InlineData(AppointmentStatuses.Cancelled)]
    [InlineData(AppointmentStatuses.Completed)]
    public void Confirm_WhenNotPending_Throws(string status) =>
        Assert.Throws<InvalidOperationException>(InStatus(status).Confirm);

    [Theory]
    [InlineData(AppointmentStatuses.Pending)]
    [InlineData(AppointmentStatuses.Confirmed)]
    public void Cancel_WhenPendingOrConfirmed_ClosesIt(string status)
    {
        var appointment = InStatus(status);

        appointment.Cancel();

        Assert.Equal(AppointmentStatuses.Cancelled, appointment.Status);
        Assert.True(appointment.IsClosed);
    }

    [Theory]
    [InlineData(AppointmentStatuses.Cancelled)]
    [InlineData(AppointmentStatuses.Completed)]
    public void Cancel_WhenClosed_Throws(string status) =>
        Assert.Throws<InvalidOperationException>(InStatus(status).Cancel);

    [Theory]
    [InlineData(AppointmentStatuses.Pending)]
    [InlineData(AppointmentStatuses.Confirmed)]
    public void Complete_WhenActive_ClosesIt(string status)
    {
        var appointment = InStatus(status);

        appointment.Complete();

        Assert.Equal(AppointmentStatuses.Completed, appointment.Status);
        Assert.True(appointment.IsClosed);
        Assert.False(appointment.CanBeCancelled);
    }

    [Theory]
    [InlineData(AppointmentStatuses.Cancelled)]
    [InlineData(AppointmentStatuses.Completed)]
    public void Complete_WhenClosed_Throws(string status) =>
        Assert.Throws<InvalidOperationException>(InStatus(status).Complete);

    [Fact]
    public void AssignTechnician_WhenActive_ReplacesEarlierTechnician()
    {
        var appointment = RequestInPerson();
        var technician = Guid.NewGuid();
        appointment.AssignTechnician(Guid.NewGuid());

        appointment.AssignTechnician(technician);

        Assert.Equal(technician, appointment.AssignedTechnicianUserId);
    }

    [Theory]
    [InlineData(AppointmentStatuses.Cancelled)]
    [InlineData(AppointmentStatuses.Completed)]
    public void AssignTechnician_WhenClosed_Throws(string status) =>
        Assert.Throws<InvalidOperationException>(() => InStatus(status).AssignTechnician(Guid.NewGuid()));
}
