using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.CompleteReferral;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Constants;

namespace OnlineConsulting.Api.Features.Referrals;

/// <summary>A referral in the staff list (names joined in from Identity); same rule as <see cref="ReferralLinks"/>.</summary>
public sealed class AdminReferralLinks : LinkProvider<AdminReferralResponse>
{
    protected override void AddLinks(AdminReferralResponse resource, HateoasLinkBuilder links)
        => links.AddCustomIf(resource.Status == ReferralStatuses.Pending && links.User.CanSend<CompleteReferralCommand>(), Rels.Complete, "CompleteReferral", HttpMethods.Post, new { id = resource.Id });
}
