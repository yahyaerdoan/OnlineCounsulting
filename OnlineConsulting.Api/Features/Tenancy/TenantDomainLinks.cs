using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.CheckTenantDomain;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.ConnectTenantDomain;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.RemoveTenantDomain;
using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Api.Features.Tenancy;

/// <summary>Connect while none is set, check while pending, delete once set.</summary>
public sealed class TenantDomainLinks : LinkProvider<TenantDomainResponse>
{
    protected override void AddLinks(TenantDomainResponse resource, HateoasLinkBuilder links)
    {
        var user = links.User;
        links.Self("GetMyTenantDomain");

        _ = links
            .AddCustomIf(resource.Host is null && user.CanSend<ConnectTenantDomainCommand>(), Rels.ConnectDomain, "ConnectTenantDomain", HttpMethods.Post)
            .AddCustomIf(resource.Status == TenantDomainStatuses.Pending && user.CanSend<CheckTenantDomainCommand>(), Rels.CheckDomain, "CheckTenantDomain", HttpMethods.Post)
            .AddCustomIf(resource.Host is not null && user.CanSend<RemoveTenantDomainCommand>(), Rels.Delete, "RemoveTenantDomain", HttpMethods.Delete);
    }
}
