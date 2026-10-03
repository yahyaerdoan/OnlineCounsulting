using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Contracts;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.WorkOrderMediaItems.AddWorkOrderMediaItem;

namespace OnlineConsulting.Api.Features.Scheduling.WorkOrders;

public sealed class WorkOrderLinks : LinkProvider<WorkOrderResponse>
{
    protected override void AddLinks(WorkOrderResponse resource, HateoasLinkBuilder links)
        => links
            .Self("GetWorkOrderByAppointmentId", new { appointmentId = resource.AppointmentId })
            .AddCustom(Rels.Appointment, "GetAppointmentById", HttpMethods.Get, new { id = resource.AppointmentId })
            .AddCustomIf(links.User.CanSend<AddWorkOrderMediaItemCommand>(), Rels.AddMedia, "AddWorkOrderMediaItem", HttpMethods.Post, new { workOrderId = resource.Id });
}
