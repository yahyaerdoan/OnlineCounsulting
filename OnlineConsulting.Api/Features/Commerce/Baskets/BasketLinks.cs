using Hateoas;
using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.CreateOrderFromBasket;

namespace OnlineConsulting.Api.Features.Commerce.Baskets;

/// <summary>Checkout needs a signed-in caller (a guest basket is merged at login); clearing and checking out need at least one item.</summary>
public sealed class BasketLinks : LinkProvider<BasketResponse>
{
    protected override void AddLinks(BasketResponse resource, HateoasLinkBuilder links)
        => links
            .Self("GetBasket")
            .AddCustomIf(resource.Items.Count > 0 && links.User.CanSend<CreateOrderFromBasketCommand>(), Rels.Checkout, "CreateOrderFromBasket", HttpMethods.Post)
            .AddCustomIf(resource.Items.Count > 0, Rels.Clear, "ClearBasket", HttpMethods.Delete);
}
