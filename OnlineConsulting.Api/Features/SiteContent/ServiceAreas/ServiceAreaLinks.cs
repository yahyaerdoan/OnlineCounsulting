using Hateoas;
using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.DeleteServiceArea;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.UpdateServiceArea;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceAreas;

public sealed class ServiceAreaLinks() : ManagedContentLinks<ServiceAreaResponse, UpdateServiceAreaCommand, DeleteServiceAreaCommand>("UpdateServiceArea", "DeleteServiceArea", resource => resource.Id)
{
    protected override void AddLinks(ServiceAreaResponse resource, HateoasLinkBuilder links)
    {
        _ = links.Self("GetServiceAreaBySlug", new { slug = resource.Slug });
        base.AddLinks(resource, links);
    }
}
