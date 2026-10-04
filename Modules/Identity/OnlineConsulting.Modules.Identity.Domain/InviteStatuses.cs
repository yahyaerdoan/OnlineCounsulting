namespace OnlineConsulting.Modules.Identity.Domain;

/// <summary>Values of <see cref="Invite.Status"/>.</summary>
public static class InviteStatuses
{
    public const string Pending = "Pending";
    public const string Accepted = "Accepted";
    public const string Revoked = "Revoked";
    public const string Expired = "Expired";
}
