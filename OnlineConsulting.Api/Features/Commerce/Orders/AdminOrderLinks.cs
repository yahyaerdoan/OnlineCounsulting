using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.RefundOrder;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

/// <summary>Links for an order in the staff list; refund only when paid.</summary>
public sealed class AdminOrderLinks : LinkProvider<AdminOrderResponse>
{
    protected override void AddLinks(AdminOrderResponse resource, HateoasLinkBuilder links)
        => links.AddCustomIf(OrderRules.CanBeRefunded(resource.PaymentStatus) && links.User.CanSend<RefundOrderCommand>(), Rels.Refund, "RefundOrder", HttpMethods.Post, new { id = resource.Id });
}
