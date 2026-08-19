namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;

public static class CustomerMembershipMessages
{
    public const string CustomerMembershipNotFoundFormat = "Customer membership {0} was not found.";
    public const string MembershipPlanNotFoundFormat = "Membership plan {0} was not found.";
    public const string AlreadyHasActiveMembership = "This customer already has an active membership.";
    public const string NoActiveMembership = "This customer has no active membership.";
    public const string PreviousAttemptNeedsSupport = "Your previous subscription attempt is in an inconsistent state and needs manual attention. Please contact support before trying again.";
    public const string PaymentSetupFailed = "We couldn't complete payment setup for your membership. Please try again in a few minutes, or contact support if the problem persists.";
}
