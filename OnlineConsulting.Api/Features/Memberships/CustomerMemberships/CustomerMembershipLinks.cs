using Hateoas;
using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.AdminCancelMembership;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.AdminReactivateMembership;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Contracts;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

/// <summary>
/// The member's own membership carries the self-service actions (routes act on "my" membership, so they take no id); staff looking at
/// someone else's membership get the admin cancel / reactivate routes. Conditions mirror the handlers: pause only while Active, resume
/// only while Paused, cancel until cancelled or already ending, reactivate only while ending (CancelAtPeriodEnd).
/// </summary>
public sealed class CustomerMembershipLinks : LinkProvider<CustomerMembershipResponse>
{
    protected override void AddLinks(CustomerMembershipResponse resource, HateoasLinkBuilder links)
    {
        var cancelled = resource.Status == CustomerMembershipStatuses.Cancelled;
        var ending = resource.CancelAtPeriodEnd && !cancelled;

        _ = links.AddCustom(Rels.Plan, "GetMembershipPlanById", HttpMethods.Get, new { id = resource.MembershipPlanId });

        if (links.User.IsUser(resource.UserId))
        {
            _ = links
                .AddIf(!cancelled, LinkRelations.Self, "GetMyMembership", HttpMethods.Get)
                .AddCustomIf(resource.Status == CustomerMembershipStatuses.Active && !resource.CancelAtPeriodEnd, Rels.ChangePlan, "ChangeMembershipPlan", HttpMethods.Post)
                .AddCustomIf(resource.Status == CustomerMembershipStatuses.Active, Rels.Pause, "PauseMembership", HttpMethods.Post)
                .AddCustomIf(resource.Status == CustomerMembershipStatuses.Paused, Rels.Resume, "ResumeMembership", HttpMethods.Post)
                .AddCustomIf(!cancelled && !resource.CancelAtPeriodEnd, Rels.Cancel, "CancelMembership", HttpMethods.Post)
                .AddCustomIf(ending, Rels.Reactivate, "ReactivateMembership", HttpMethods.Post)
                .AddCustomIf(cancelled && resource.PlanIsActive == true, Rels.Subscribe, "SubscribeToMembership", HttpMethods.Post);
            return;
        }

        var id = new { id = resource.Id };
        _ = links
            .AddCustomIf(!cancelled && links.User.CanSend<AdminCancelMembershipCommand>(), Rels.Cancel, "AdminCancelMembership", HttpMethods.Post, id)
            .AddCustomIf(ending && links.User.CanSend<AdminReactivateMembershipCommand>(), Rels.Reactivate, "AdminReactivateMembership", HttpMethods.Post, id);
    }
}
