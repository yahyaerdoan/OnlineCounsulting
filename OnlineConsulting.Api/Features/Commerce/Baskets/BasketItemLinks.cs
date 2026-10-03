using Hateoas;
using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Contracts;

namespace OnlineConsulting.Api.Features.Commerce.Baskets;

public sealed class BasketItemLinks : LinkProvider<BasketItemResponse>
{
    protected override void AddLinks(BasketItemResponse resource, HateoasLinkBuilder links)
        => links
            .Add(LinkRelations.Edit, "SetBasketItemQuantity", HttpMethods.Put, new { id = resource.Id })
            .AddCustom(Rels.Remove, "RemoveBasketItem", HttpMethods.Delete, new { id = resource.Id })
            .AddCustom(Rels.Service, "GetServiceById", HttpMethods.Get, new { id = resource.ServiceId });
}
