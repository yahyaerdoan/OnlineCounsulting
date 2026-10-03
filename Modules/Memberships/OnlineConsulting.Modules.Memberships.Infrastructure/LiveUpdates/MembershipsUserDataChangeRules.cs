using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.SharedKernel.LiveUpdates;

namespace OnlineConsulting.Modules.Memberships.Infrastructure.LiveUpdates;

/// <summary>Any change to a customer's membership (subscribe, pause, resume, cancel, provider webhooks) signals its owner.</summary>
public static class MembershipsUserDataChangeRules
{
    public static void Configure(UserDataChangeRuleSet rules) => rules
        .ForUserProperties<CustomerMembership>(UserDataTopics.Membership, nameof(CustomerMembership.UserId));
}
