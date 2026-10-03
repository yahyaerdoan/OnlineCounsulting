using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Scheduling.Application.Features.AppointmentMediaItems.Contracts;

namespace OnlineConsulting.Api.Features.Scheduling.AppointmentMediaItems;

public sealed class AppointmentMediaItemLinks : LinkProvider<AppointmentMediaItemResponse>
{
    protected override void AddLinks(AppointmentMediaItemResponse resource, HateoasLinkBuilder links)
        => links.AddCustom(Rels.Media, "GetMediaAsset", HttpMethods.Get, new { id = resource.MediaAssetId });
}
