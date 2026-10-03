using Hateoas;
using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.SubscribeToMembership;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Contracts;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.SetMembershipPlanActive;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.UpdateMembershipPlan;

namespace OnlineConsulting.Api.Features.Memberships.MembershipPlans;

/// <summary>A signed-in visitor can subscribe to an active plan; staff can edit any plan and show or hide it.</summary>
public sealed class MembershipPlanLinks : LinkProvider<MembershipPlanResponse>
{
    protected override void AddLinks(MembershipPlanResponse resource, HateoasLinkBuilder links)
        => links
            .Self("GetMembershipPlanById", new { id = resource.Id })
            .AddCustomIf(resource.IsActive && links.User.CanSend<SubscribeToMembershipCommand>(), Rels.Subscribe, "SubscribeToMembership", HttpMethods.Post)
            .AddIf(links.User.CanSend<UpdateMembershipPlanCommand>(), LinkRelations.Edit, "UpdateMembershipPlan", HttpMethods.Put, new { id = resource.Id })
            .AddCustomIf(links.User.CanSend<SetMembershipPlanActiveCommand>(), Rels.SetActive, "SetMembershipPlanActive", HttpMethods.Put, new { id = resource.Id },
                resource.IsActive ? "Hide from customers" : "Show to customers");
}
