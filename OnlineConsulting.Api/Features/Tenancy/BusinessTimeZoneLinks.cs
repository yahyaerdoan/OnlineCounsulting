using Hateoas.AspNetCore;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;

namespace OnlineConsulting.Api.Features.Tenancy;

public sealed class BusinessTimeZoneLinks : LinkProvider<BusinessTimeZoneResponse>
{
    protected override void AddLinks(BusinessTimeZoneResponse resource, HateoasLinkBuilder links) => links.Self("GetBusinessTimeZone");
}
