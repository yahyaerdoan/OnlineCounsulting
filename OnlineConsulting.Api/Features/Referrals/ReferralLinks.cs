using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.CompleteReferral;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Contracts;
using OnlineConsulting.Modules.Referrals.Domain;

namespace OnlineConsulting.Api.Features.Referrals;

public sealed class ReferralLinks : LinkProvider<ReferralResponse>
{
    protected override void AddLinks(ReferralResponse resource, HateoasLinkBuilder links)
        => links.AddCustomIf(resource.Status == ReferralStatuses.Pending && links.User.CanSend<CompleteReferralCommand>(), Rels.Complete, "CompleteReferral", HttpMethods.Post, new { id = resource.Id });
}
