using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.AssignTechnician;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.CancelAppointmentByStaff;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.ConfirmAppointment;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Constants;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Contracts;

namespace OnlineConsulting.Api.Features.Scheduling.Appointments;

/// <summary>
/// The visit's owner can cancel it while Pending or Confirmed. Staff can confirm a Pending visit, assign a technician until it is closed
/// and cancel it while Pending or Confirmed, each when their permissions allow it. A completed visit links its work order.
/// </summary>
public sealed class AppointmentLinks : LinkProvider<AppointmentResponse>
{
    protected override void AddLinks(AppointmentResponse resource, HateoasLinkBuilder links)
    {
        var id = new { id = resource.Id };
        var cancellable = resource.Status is AppointmentStatuses.Pending or AppointmentStatuses.Confirmed;
        var closed = resource.Status is AppointmentStatuses.Cancelled or AppointmentStatuses.Completed;
        var isOwner = links.User.IsUser(resource.UserId);

        _ = links
            .Self("GetAppointmentById", id)
            .AddCustomIf(resource.Status == AppointmentStatuses.Completed, Rels.WorkOrder, "GetWorkOrderByAppointmentId", HttpMethods.Get, new { appointmentId = resource.Id })
            .AddCustomIf(isOwner && cancellable, Rels.Cancel, "CancelAppointment", HttpMethods.Post, id)
            .AddCustomIf(resource.Status == AppointmentStatuses.Pending && links.User.CanSend<ConfirmAppointmentCommand>(), Rels.Confirm, "ConfirmAppointment", HttpMethods.Post, id)
            .AddCustomIf(!closed && links.User.CanSend<AssignTechnicianCommand>(), Rels.AssignTechnician, "AssignTechnician", HttpMethods.Post, id)
            .AddCustomIf(!isOwner && cancellable && links.User.CanSend<CancelAppointmentByStaffCommand>(), Rels.Cancel, "CancelAppointmentByStaff", HttpMethods.Post, id);
    }
}
