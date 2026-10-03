using Hateoas.AspNetCore;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;

namespace OnlineConsulting.Api.Features.Tenancy;

public sealed class TenantDetailLinks : LinkProvider<TenantDetailResponse>
{
    protected override void AddLinks(TenantDetailResponse resource, HateoasLinkBuilder links)
        => TenantLinks.AddTenantLinks(links, resource.Id, resource.Status);
}
