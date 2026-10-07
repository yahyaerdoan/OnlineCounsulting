using Hateoas.AspNetCore;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;

namespace OnlineConsulting.Api.Features.Tenancy;

public sealed class TenantBrandingLinks : LinkProvider<TenantBrandingResponse>
{
    protected override void AddLinks(TenantBrandingResponse resource, HateoasLinkBuilder links) => links.Self("GetTenantBranding");
}
