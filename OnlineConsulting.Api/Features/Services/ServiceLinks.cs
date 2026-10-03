using Hateoas;
using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Services.Application.Features.Services.Contracts;
using OnlineConsulting.Modules.Services.Application.Features.Services.DeleteService;
using OnlineConsulting.Modules.Services.Application.Features.Services.UpdateService;

namespace OnlineConsulting.Api.Features.Services;

public sealed class ServiceLinks : LinkProvider<ServiceResponse>
{
    protected override void AddLinks(ServiceResponse resource, HateoasLinkBuilder links)
        => links
            .Self("GetServiceById", new { id = resource.Id })
            .AddIf(links.User.CanSend<UpdateServiceCommand>(), LinkRelations.Edit, "UpdateService", HttpMethods.Put, new { id = resource.Id })
            .AddCustomIf(links.User.CanSend<DeleteServiceCommand>(), Rels.Delete, "DeleteService", HttpMethods.Delete, new { id = resource.Id });
}
