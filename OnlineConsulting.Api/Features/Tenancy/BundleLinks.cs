using Hateoas;
using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.UpdateBundle;

namespace OnlineConsulting.Api.Features.Tenancy;

public sealed class BundleLinks : LinkProvider<BundleAdminResponse>
{
    protected override void AddLinks(BundleAdminResponse resource, HateoasLinkBuilder links)
        => links
            .Self("GetBundleById", new { id = resource.Id })
            .AddIf(links.User.CanSend<UpdateBundleCommand>(), LinkRelations.Edit, "UpdateBundle", HttpMethods.Put, new { id = resource.Id });
}
