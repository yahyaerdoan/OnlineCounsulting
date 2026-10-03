using Hateoas;
using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.UpdateModuleOffering;

namespace OnlineConsulting.Api.Features.Tenancy;

public sealed class ModuleOfferingLinks : LinkProvider<ModuleOfferingAdminResponse>
{
    protected override void AddLinks(ModuleOfferingAdminResponse resource, HateoasLinkBuilder links)
        => links
            .Self("GetModuleOfferingById", new { id = resource.Id })
            .AddIf(links.User.CanSend<UpdateModuleOfferingCommand>(), LinkRelations.Edit, "UpdateModuleOffering", HttpMethods.Put, new { id = resource.Id });
}
