namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;

/// <summary>The admin subscriber list's tabs; unlike CustomerMembershipStatuses these can span statuses ("Ending" is any not-yet-cancelled
/// membership set to cancel at period end).</summary>
public static class CustomerMembershipListViews
{
    public const string Active = "Active";
    public const string Ending = "Ending";
    public const string NeedsAttention = "NeedsAttention";
    public const string Paused = "Paused";
    public const string Cancelled = "Cancelled";
}
