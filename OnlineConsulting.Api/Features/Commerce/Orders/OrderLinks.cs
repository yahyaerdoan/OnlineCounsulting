using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

/// <summary>Links for the caller's own order; pay, cancel and change addresses only while it awaits payment.</summary>
public sealed class OrderLinks : LinkProvider<OrderResponse>
{
    protected override void AddLinks(OrderResponse resource, HateoasLinkBuilder links)
    {
        var unpaid = OrderRules.IsAwaitingPayment(resource.PaymentStatus, resource.OrderStatus);

        _ = links
            .Self("GetOrderDetail", new { id = resource.Id })
            .AddCustomIf(unpaid, Rels.Pay, "ResumeOrderPayment", HttpMethods.Get, new { id = resource.Id })
            .AddCustomIf(unpaid, Rels.ChangeAddresses, "UpdatePendingOrderAddresses", HttpMethods.Put, new { id = resource.Id })
            .AddCustomIf(unpaid, Rels.Cancel, "CancelPendingOrder", HttpMethods.Post, new { id = resource.Id });
    }
}
