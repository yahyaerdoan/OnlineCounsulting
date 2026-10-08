namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;

public static class CustomerMembershipMessages
{
    public const string CustomerMembershipNotFoundFormat = "Customer membership {0} was not found.";
    public const string MembershipPlanNotFoundFormat = "Membership plan {0} was not found.";
    public const string AlreadyHasActiveMembership = "This customer already has an active membership.";
    public const string NoActiveMembership = "This customer has no active membership.";
    public const string AlreadyCancelled = "This membership is already cancelled.";
    public const string AlreadyOnThisPlan = "You are already subscribed to this plan.";
    public const string PlanChangeFailed = "We couldn't change your plan with the payment provider. Please try again in a few minutes.";
    public const string NotPausable = "Only active memberships can be paused.";
    public const string NotResumable = "Only paused memberships can be resumed.";
    public const string AlreadyEnding = "This membership is already set to end.";
    public const string ReactivateBeforePlanChange = "Your membership is set to end. Reactivate it before changing plans.";
    public const string NotReactivatable = "This membership isn't set to end, so there is nothing to undo.";
    public const string ReactivateFailed = "We couldn't restore your membership with the payment provider. Please try again in a few minutes.";
    public const string NoPreviousMembership = "There is no ended membership to rejoin.";
    public const string PauseFailed = "We couldn't pause your membership with the payment provider. Please try again in a few minutes.";
    public const string ResumeFailed = "We couldn't resume your membership with the payment provider. Please try again in a few minutes.";
    public const string PreviousAttemptNeedsSupport = "Your previous subscription attempt is in an inconsistent state and needs manual attention. Please contact support before trying again.";
    public const string CreditAppliedReason = "Applied to membership subscription";
    public const string CreditReturnedReason = "Returned: membership payment setup failed";
    public const string InsufficientCredit = "Your account credit balance changed and no longer covers the requested amount. Please review it and try again.";
    public const string PaymentSetupFailed = "We couldn't complete payment setup for your membership. Please try again in a few minutes, or contact support if the problem persists.";
}
