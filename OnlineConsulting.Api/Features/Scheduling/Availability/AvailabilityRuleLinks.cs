using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Scheduling.Application.Features.Availability.Contracts;
using OnlineConsulting.Modules.Scheduling.Application.Features.Availability.DeleteAvailabilityRule;

namespace OnlineConsulting.Api.Features.Scheduling.Availability;

public sealed class AvailabilityRuleLinks : LinkProvider<AvailabilityRuleResponse>
{
    protected override void AddLinks(AvailabilityRuleResponse resource, HateoasLinkBuilder links)
        => links.AddCustomIf(links.User.CanSend<DeleteAvailabilityRuleCommand>(), Rels.Delete, "DeleteAvailabilityRule", HttpMethods.Delete, new { id = resource.Id });
}
