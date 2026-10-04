using Hateoas;
using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.CancelTenant;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.ChangeTenantTimeZone;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.GetTenantById;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.ReactivateTenant;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.SuspendTenant;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Api.Features.Tenancy;

/// <summary>Staff actions follow the handlers (suspend while Active or PastDue, reactivate while Suspended, cancel until Cancelled); a tenant admin viewing
/// their own tenant can change its time zone.</summary>
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
            .AddCustomIf(TenantRules.CanBeSuspended(status) && user.CanSend<SuspendTenantCommand>(), Rels.Suspend, "SuspendTenant", HttpMethods.Post, id)
            .AddCustomIf(TenantRules.CanBeReactivated(status) && user.CanSend<ReactivateTenantCommand>(), Rels.Reactivate, "ReactivateTenant", HttpMethods.Post, id)
            .AddCustomIf(TenantRules.CanBeCancelled(status) && user.CanSend<CancelTenantCommand>(), Rels.Cancel, "CancelTenant", HttpMethods.Post, id)
            .AddCustomIf(user.FindFirst(TenantProvider.TenantClaimType)?.Value == tenantId.ToString() && user.CanSend<ChangeTenantTimeZoneCommand>(),
                Rels.ChangeTimeZone, "ChangeTenantTimeZone", HttpMethods.Put);
    }
}
