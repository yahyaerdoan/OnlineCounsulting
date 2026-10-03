using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Contracts;

namespace OnlineConsulting.Api.Features.Scheduling.WorkOrders.WorkOrderMediaItems;

public sealed class WorkOrderMediaItemLinks : LinkProvider<WorkOrderMediaItemResponse>
{
    protected override void AddLinks(WorkOrderMediaItemResponse resource, HateoasLinkBuilder links)
        => links.AddCustom(Rels.Media, "GetMediaAsset", HttpMethods.Get, new { id = resource.MediaAssetId });
}
