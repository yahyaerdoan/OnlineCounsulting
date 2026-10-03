using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.LiveUpdates;

namespace OnlineConsulting.Modules.Identity.Infrastructure.LiveUpdates;

/// <summary>Profile signals only for fields a user actually sees (name, email, phone, photo, active flag) - sign-ins also
/// touch the user row (security stamp, lockout counters) and must not refresh every open screen. Role changes signal too;
/// a new or read inbox notification refreshes the bell count.</summary>
public static class IdentityUserDataChangeRules
{
    public static void Configure(UserDataChangeRuleSet rules) => rules
        .For<User>(UserDataTopics.Profile, (entry, _) => ValueTask.FromResult<IEnumerable<Guid>>([entry.Entity.Id]),
            nameof(User.FirstName), nameof(User.LastName), nameof(User.Email), nameof(User.UserName),
            nameof(User.PhoneNumber), nameof(User.ImageUrl), nameof(User.IsActive))
        .ForUserProperties<IdentityUserRole<Guid>>(UserDataTopics.Profile, nameof(IdentityUserRole<Guid>.UserId))
        .ForUserProperties<UserNotification>(UserDataTopics.Notifications, nameof(UserNotification.UserId));
}
