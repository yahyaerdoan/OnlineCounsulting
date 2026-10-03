using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.RefundOrder;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

/// <summary>An order in the staff list. Only a paid order can be refunded.</summary>
public sealed class AdminOrderLinks : LinkProvider<AdminOrderResponse>
{
    protected override void AddLinks(AdminOrderResponse resource, HateoasLinkBuilder links)
        => links.AddCustomIf(resource.PaymentStatus == PaymentStatuses.Paid && links.User.CanSend<RefundOrderCommand>(), Rels.Refund, "RefundOrder", HttpMethods.Post, new { id = resource.Id });
}
