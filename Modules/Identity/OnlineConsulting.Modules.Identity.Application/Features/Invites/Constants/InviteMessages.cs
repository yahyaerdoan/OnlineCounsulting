namespace OnlineConsulting.Modules.Identity.Application.Features.Invites.Constants;

public static class InviteMessages
{
    public const string RoleNotFound = "The specified role does not exist.";
    public const string RoleNotInvitable = "Super Admin cannot be granted through an invite.";
    public const string InviteAlreadyPending = "An active invite has already been sent to this email address.";
    public const string EmailAlreadyRegistered = "This email address is already registered.";
    public const string InviteNotFound = "This invite link is invalid.";
    public const string InviteNotUsable = "This invite has already been used or was revoked.";
    public const string InviteExpired = "This invite link has expired. Please ask your admin to send a new one.";
    public const string InviteCancelled = "The invite has been cancelled.";
    public const string InviteNotCancellable = "Only a pending invite can be cancelled.";
    public const string NotAuthorizedForOtherTenant = "You are not authorized to manage invites of another tenant.";
}
