using OnlineConsulting.SharedKernel.CurrentUser;

namespace OnlineConsulting.Api.Common;

public static class CurrentUserExtensions
{
    /// <summary>The signed-in user's id from the token, for endpoints that require authorization; no database lookup.</summary>
    public static Guid RequiredId(this ICurrentUserAccessor currentUser) =>
        currentUser.Id ?? throw new InvalidOperationException("An endpoint that requires authorization ran without a user id claim.");

    /// <summary>The signed-in user's email from the token, for endpoints that require authorization; no database lookup.</summary>
    public static string RequiredEmail(this ICurrentUserAccessor currentUser) =>
        currentUser.Email ?? throw new InvalidOperationException("An endpoint that requires authorization ran without an email claim.");
}
