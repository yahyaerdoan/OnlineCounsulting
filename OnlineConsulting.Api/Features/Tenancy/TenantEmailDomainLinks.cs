using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.CheckTenantEmailDomain;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.ConnectTenantEmailDomain;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.RemoveTenantEmailDomain;
using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Api.Features.Tenancy;

/// <summary>Connect while none is set, check while pending, delete once set.</summary>
public sealed class TenantEmailDomainLinks : LinkProvider<TenantEmailDomainResponse>
{
    protected override void AddLinks(TenantEmailDomainResponse resource, HateoasLinkBuilder links)
    {
        var user = links.User;
        links.Self("GetMyTenantEmailDomain");

        _ = links
            .AddCustomIf(resource.Domain is null && user.CanSend<ConnectTenantEmailDomainCommand>(), Rels.ConnectEmailDomain, "ConnectTenantEmailDomain", HttpMethods.Post)
            .AddCustomIf(resource.Status == TenantEmailDomainStatuses.Pending && user.CanSend<CheckTenantEmailDomainCommand>(), Rels.CheckEmailDomain, "CheckTenantEmailDomain", HttpMethods.Post)
            .AddCustomIf(resource.Domain is not null && user.CanSend<RemoveTenantEmailDomainCommand>(), Rels.Delete, "RemoveTenantEmailDomain", HttpMethods.Delete);
    }
}
