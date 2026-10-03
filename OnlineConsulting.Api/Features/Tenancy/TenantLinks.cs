using Hateoas;
using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.CancelTenant;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.GetTenantById;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.ReactivateTenant;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.SuspendTenant;
using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Api.Features.Tenancy;

/// <summary>Platform staff only. Suspend while Active or PastDue, reactivate while Suspended, cancel until Cancelled (mirrors the handlers).</summary>
public sealed class TenantLinks : LinkProvider<TenantSummaryResponse>
{
    protected override void AddLinks(TenantSummaryResponse resource, HateoasLinkBuilder links)
        => AddTenantLinks(links, resource.Id, resource.Status);

    /// <summary>Shared with <see cref="TenantDetailLinks"/>.</summary>
    internal static void AddTenantLinks(HateoasLinkBuilder links, Guid tenantId, string status)
    {
        var id = new { tenantId };
        var user = links.User;

        _ = links
            .AddIf(user.CanSend<GetTenantByIdQuery>(), LinkRelations.Self, "GetTenantById", HttpMethods.Get, id)
            .AddCustomIf(status is TenantStatuses.Active or TenantStatuses.PastDue && user.CanSend<SuspendTenantCommand>(), Rels.Suspend, "SuspendTenant", HttpMethods.Post, id)
            .AddCustomIf(status == TenantStatuses.Suspended && user.CanSend<ReactivateTenantCommand>(), Rels.Reactivate, "ReactivateTenant", HttpMethods.Post, id)
            .AddCustomIf(status != TenantStatuses.Cancelled && user.CanSend<CancelTenantCommand>(), Rels.Cancel, "CancelTenant", HttpMethods.Post, id);
    }
}
