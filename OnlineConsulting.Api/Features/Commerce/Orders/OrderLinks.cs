using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

/// <summary>The caller's own order. Pay, cancel and change-addresses mirror the handlers: only an unpaid, uncancelled order.</summary>
public sealed class OrderLinks : LinkProvider<OrderResponse>
{
    protected override void AddLinks(OrderResponse resource, HateoasLinkBuilder links)
    {
        var unpaid = resource.PaymentStatus == PaymentStatuses.Pending && resource.OrderStatus != OrderStatuses.Cancelled;

        _ = links
            .Self("GetOrderDetail", new { id = resource.Id })
            .AddCustomIf(unpaid, Rels.Pay, "ResumeOrderPayment", HttpMethods.Get, new { id = resource.Id })
            .AddCustomIf(unpaid, Rels.ChangeAddresses, "UpdatePendingOrderAddresses", HttpMethods.Put, new { id = resource.Id })
            .AddCustomIf(resource.PaymentStatus == PaymentStatuses.Pending, Rels.Cancel, "CancelPendingOrder", HttpMethods.Post, new { id = resource.Id });
    }
}
