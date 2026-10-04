using Microsoft.Extensions.Logging;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.Modules.Scheduling.Application.Common.Templates;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Domain;
using OnlineConsulting.SharedKernel.Catalog;
using OnlineConsulting.SharedKernel.Identity;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Notifications;

/// <summary>Customer emails go through the Scheduling outbox (retried by the email dispatcher); pushes go through the inbox-recording sender,
/// so the bell shows them even when no device is registered.</summary>
public class AppointmentNotifier(IEmailOutboxWriter<ISchedulingOutboxModule> outboxWriter,
                                 IEmailTemplate<AppointmentUpdateEmailModel> emailTemplate,
                                 IPushNotificationSender pushSender,
                                 IUserContactReader contactReader,
                                 IStaffDirectory staffDirectory,
                                 IServiceCatalogReader catalogReader,
                                 ITenantTimeZoneReader timeZoneReader,
                                 ILogger<AppointmentNotifier> logger) : IAppointmentNotifier
{
    private static readonly string[] DispatcherPermissions = [SchedulingOperationClaims.Admin, SchedulingOperationClaims.Write, SchedulingOperationClaims.Update];

    public async Task RequestedAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        var visit = await DescribeAsync(appointment, cancellationToken);

        await EmailCustomerAsync(appointment, visit, AppointmentUpdateKind.Requested, cancellationToken: cancellationToken);

        await PushAsync(appointment.UserId, appointment, "Booking request received",
            $"We got your request for {visit.ServiceTitle} on {visit.When}. We'll let you know once it's confirmed.", cancellationToken);

        var where = IsOnline(appointment) ? $" (online: {appointment.Topic})" : "";

        await PushStaffAsync(appointment, "New appointment request",
            $"{visit.CustomerName} requested {visit.ServiceTitle} on {visit.When}{where}.", cancellationToken);
    }

    public async Task ConfirmedAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        var visit = await DescribeAsync(appointment, cancellationToken);

        await EmailCustomerAsync(appointment, visit, AppointmentUpdateKind.Confirmed, cancellationToken: cancellationToken);

        await PushAsync(appointment.UserId, appointment, "Your visit is confirmed", $"{visit.ServiceTitle} on {visit.When}. See you then!", cancellationToken);
    }

    public async Task CancelledByCustomerAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        var visit = await DescribeAsync(appointment, cancellationToken);

        await EmailCustomerAsync(appointment, visit, AppointmentUpdateKind.CancelledByCustomer, cancellationToken: cancellationToken);

        await PushTechnicianAsync(appointment.AssignedTechnicianUserId, appointment, "Job cancelled",
            $"{visit.CustomerName} cancelled {visit.ServiceTitle} on {visit.When}. It's been removed from your schedule.", cancellationToken);

        await PushStaffAsync(appointment, "Appointment cancelled by customer",
            $"{visit.CustomerName} cancelled {visit.ServiceTitle} on {visit.When}.", cancellationToken);
    }

    public async Task CancelledByStaffAsync(Appointment appointment, string? reason, CancellationToken cancellationToken = default)
    {
        var visit = await DescribeAsync(appointment, cancellationToken);

        var reasonText = string.IsNullOrWhiteSpace(reason) ? "" : $" Reason: {reason.Trim()}";

        await EmailCustomerAsync(appointment, visit, AppointmentUpdateKind.CancelledByStaff, reason: reason, cancellationToken: cancellationToken);

        await PushAsync(appointment.UserId, appointment, "Your appointment was cancelled",
            $"We had to cancel {visit.ServiceTitle} on {visit.When}.{reasonText}", cancellationToken);

        await PushTechnicianAsync(appointment.AssignedTechnicianUserId, appointment, "Job cancelled",
            $"{visit.ServiceTitle} for {visit.CustomerName} on {visit.When} was cancelled. It's been removed from your schedule.", cancellationToken);
    }

    public async Task TechnicianAssignedAsync(Appointment appointment, Guid? previousTechnicianUserId, CancellationToken cancellationToken = default)
    {
        var visit = await DescribeAsync(appointment, cancellationToken);

        var technician = appointment.AssignedTechnicianUserId is { } technicianId ? await contactReader.GetContactAsync(technicianId, cancellationToken) : null;
        var technicianName = technician?.FirstName is { Length: > 0 } firstName ? firstName : "A technician";

        await EmailCustomerAsync(appointment, visit, AppointmentUpdateKind.TechnicianAssigned, technician?.FullName, cancellationToken: cancellationToken);

        await PushAsync(appointment.UserId, appointment, "Technician assigned",
            $"{technicianName} will handle your {visit.ServiceTitle} visit on {visit.When}.", cancellationToken);

        var address = string.IsNullOrWhiteSpace(appointment.ServiceAddress) ? "" : $" at {appointment.ServiceAddress}";

        await PushTechnicianAsync(appointment.AssignedTechnicianUserId, appointment, "New job assigned",
            $"{visit.ServiceTitle} for {visit.CustomerName} on {visit.When}{address}.", cancellationToken);

        if (previousTechnicianUserId is { } previousId && previousId != appointment.AssignedTechnicianUserId)
        {
            await PushTechnicianAsync(previousId, appointment, "Job reassigned",
                $"{visit.ServiceTitle} for {visit.CustomerName} on {visit.When} was reassigned to another technician.", cancellationToken);
        }
    }

    public async Task CompletedAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        var visit = await DescribeAsync(appointment, cancellationToken);

        await EmailCustomerAsync(appointment, visit, AppointmentUpdateKind.Completed, cancellationToken: cancellationToken);

        await PushAsync(appointment.UserId, appointment, "Service complete",
            $"Your {visit.ServiceTitle} visit is done. Thanks for choosing us!", cancellationToken);
    }

    private sealed record VisitDescription(UserContact? Customer, string CustomerName, string ServiceTitle, DateTimeOffset LocalStart, DateTimeOffset LocalEnd)
    {
        public string When => AppointmentTimeText.When(LocalStart);
    }

    private async Task<VisitDescription> DescribeAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        var customer = await contactReader.GetContactAsync(appointment.UserId, cancellationToken);

        var serviceTitle = appointment.ServiceId is { } serviceId && await catalogReader.GetAsync(serviceId, cancellationToken) is { } service
            ? service.Title
            : "Consultation";

        var zone = await timeZoneReader.GetAsync(appointment.TenantId, cancellationToken);

        return new VisitDescription(customer, customer?.FullName is { Length: > 0 } name ? name : "A customer", serviceTitle,
            TimeZoneInfo.ConvertTime(appointment.ScheduledStart, zone), TimeZoneInfo.ConvertTime(appointment.ScheduledEnd, zone));
    }

    private async Task EmailCustomerAsync(Appointment appointment, VisitDescription visit, AppointmentUpdateKind kind, string? technicianName = null, string? reason = null, CancellationToken cancellationToken = default)
    {
        if (visit.Customer?.Email is not { Length: > 0 } email)
        {
            logger.LogWarning("Appointment {AppointmentId}: no email on file for customer {UserId}, skipping the {Kind} email.", appointment.Id, appointment.UserId, kind);

            return;
        }

        var model = new AppointmentUpdateEmailModel(kind, visit.Customer.FirstName, visit.ServiceTitle, visit.LocalStart, visit.LocalEnd,
            appointment.ServiceAddress, technicianName, reason, IsOnline(appointment), appointment.Topic);

        try
        {
            await outboxWriter.EnqueueAsync(email, emailTemplate.Subject(model), emailTemplate.Build(model), sourceReference: $"Appointment:{appointment.Id}:{kind}", cancellationToken: cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Appointment {AppointmentId}: queuing the {Kind} email failed.", appointment.Id, kind);
        }
    }

    private static bool IsOnline(Appointment appointment) => appointment.MeetingType == AppointmentMeetingTypes.Online;

    private Task PushTechnicianAsync(Guid? technicianUserId, Appointment appointment, string title, string body, CancellationToken cancellationToken) =>
        technicianUserId is { } id ? PushAsync(id, appointment, title, body, cancellationToken) : Task.CompletedTask;

    private async Task PushStaffAsync(Appointment appointment, string title, string body, CancellationToken cancellationToken)
    {
        IReadOnlyList<Guid> staffIds;
        try
        {
            staffIds = await staffDirectory.GetUserIdsWithAnyPermissionAsync(appointment.TenantId, DispatcherPermissions, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Appointment {AppointmentId}: looking up the scheduling team failed.", appointment.Id);

            return;
        }

        foreach (var staffId in staffIds.Where(id => id != appointment.UserId))
        {
            await PushAsync(staffId, appointment, title, body, cancellationToken);
        }
    }

    private async Task PushAsync(Guid userId, Appointment appointment, string title, string body, CancellationToken cancellationToken)
    {
        try
        {
            await pushSender.SendToUserAsync(userId, title, body, new Dictionary<string, string> { ["appointmentId"] = appointment.Id.ToString() }, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Appointment {AppointmentId}: sending '{Title}' to user {UserId} failed.", appointment.Id, title, userId);
        }
    }
}
