using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.DeleteFaqItem;
using OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.UpdateFaqItem;

namespace OnlineConsulting.Api.Features.SiteContent.FaqItems;

public sealed class FaqItemLinks() : ManagedContentLinks<FaqItemResponse, UpdateFaqItemCommand, DeleteFaqItemCommand>("UpdateFaqItem", "DeleteFaqItem", resource => resource.Id)
{
    protected override void AddLinks(FaqItemResponse resource, HateoasLinkBuilder links)
    {
        base.AddLinks(resource, links);
        _ = links.AddCustom(Rels.Service, "GetServiceById", HttpMethods.Get, new { id = resource.ServiceId });
    }
}
